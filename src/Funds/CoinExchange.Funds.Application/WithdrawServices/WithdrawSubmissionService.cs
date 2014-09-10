using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.WithdrawServices
{
    /// <summary>
    /// Service to store and submit withdrawals and keeping track of the time interval after which withdrawals are passed to the 
    /// CoinClientService
    /// </summary>
    public class WithdrawSubmissionService : IWithdrawSubmissionService
    {
        private IFundsPersistenceRepository _fundsPersistenceRepository;
        private IWithdrawRepository _withdrawRepository;
        private ICoinClientService _coinClientService;

        private Dictionary<Timer, Withdraw> _withdrawTimersDictionary = new Dictionary<Timer, Withdraw>();

        public event Action<Withdraw> WithdrawExecuted;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public WithdrawSubmissionService(IFundsPersistenceRepository fundsPersistenceRepository, IWithdrawRepository withdrawRepository, ICoinClientService coinClientService)
        {
            _fundsPersistenceRepository = fundsPersistenceRepository;
            _withdrawRepository = withdrawRepository;
            _coinClientService = coinClientService;
        }

        /// <summary>
        /// Saveas a withdraw to database and starts teh timer after which the withrawal is submitted to CoinClientService
        /// </summary>
        /// <param name="withdrawId"></param>
        /// <returns></returns>
        public bool CommitWithdraw(string withdrawId)
        {
            Withdraw withdraw = _withdrawRepository.GetWithdrawByWithdrawId(withdrawId);
            if (withdraw != null)
            {
                Timer timer = new Timer(Convert.ToDouble(ConfigurationManager.AppSettings.Get("WithdrawSubmitInterval")));
                timer.Elapsed += OnTimerElapsed;
                timer.Enabled = true;
                _withdrawTimersDictionary.Add(timer, withdraw);
                return true;
            }
            throw new InstanceNotFoundException(string.Format("No withdraw found for Withdraw ID = {0}", withdrawId));
        }

        /// <summary>
        /// Cancels the withdraw with the given WithdrawId
        /// </summary>
        /// <param name="withdrawId"></param>
        /// <returns></returns>
        public bool CancelWithdraw(string withdrawId)
        {
            Timer timerKey = null;
            foreach (KeyValuePair<Timer, Withdraw> keyValuePair in _withdrawTimersDictionary)
            {
                if (keyValuePair.Value.WithdrawId == withdrawId)
                {
                    timerKey = keyValuePair.Key;
                }
            }
            if (timerKey != null)
            {
                timerKey.Stop();
                timerKey.Dispose();
                _withdrawTimersDictionary.Remove(timerKey);
                Withdraw withdraw = _withdrawRepository.GetWithdrawByWithdrawId(withdrawId);
                if (withdraw != null)
                {
                    withdraw.StatusCancelled();
                    _fundsPersistenceRepository.SaveOrUpdate(withdraw);
                    return true;
                }
                throw new InvalidOperationException(string.Format("No withdraw found in the repository for Withdraw ID: {0}",
                    withdrawId));
            }
            throw new InstanceNotFoundException(string.Format("No withdraw found in memory for Withdraw ID: {0}",
                    withdrawId));
        }

        /// <summary>
        /// Handler when the timer has been elapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="elapsedEventArgs"></param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Timer timer = sender as Timer;
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                Withdraw withdraw = null;
                _withdrawTimersDictionary.TryGetValue(timer, out withdraw);
                if (withdraw != null)
                {
                    string transactionId = _coinClientService.CommitWithdraw(withdraw.BitcoinAddress.Value, withdraw.Amount);
                    withdraw.SetTransactionId(transactionId);
                    _fundsPersistenceRepository.SaveOrUpdate(withdraw);
                    if (WithdrawExecuted != null)
                    {
                        WithdrawExecuted(withdraw);
                    }
                    _withdrawTimersDictionary.Remove(timer);
                }
            }
        }
    }
}
