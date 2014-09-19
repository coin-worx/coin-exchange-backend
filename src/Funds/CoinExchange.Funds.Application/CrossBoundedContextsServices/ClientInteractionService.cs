using System;
using System.Collections.Generic;
using System.Configuration;
using System.Management.Instrumentation;
using System.Timers;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.Repositories;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Service to store and submit withdrawals and keeping track of the time interval after which withdrawals are passed to the 
    /// CoinClientService
    /// </summary>
    public class ClientInteractionService : IClientInteractionService
    {
        private IFundsPersistenceRepository _fundsPersistenceRepository;
        private IWithdrawRepository _withdrawRepository;        
        private ICoinClientService _bitcoinClientService;
        private ICoinClientService _litecoinClientService;

        private Dictionary<Timer, Withdraw> _withdrawTimersDictionary = new Dictionary<Timer, Withdraw>();

        private double _withdrawSubmissioInterval =
            Convert.ToDouble(ConfigurationManager.AppSettings.Get("WithdrawSubmitInterval"));

        public event Action<Withdraw> WithdrawExecuted;
        public event Action<string, List<Tuple<string, string, decimal, string>>> DepositArrived;
        public event Action<string, int> DepositConfirmed;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ClientInteractionService(IFundsPersistenceRepository fundsPersistenceRepository, IWithdrawRepository withdrawRepository, 
            ICoinClientService bitcoinClientService, ICoinClientService litecoinClientService)
        {
            _fundsPersistenceRepository = fundsPersistenceRepository;
            _withdrawRepository = withdrawRepository;
            _bitcoinClientService = bitcoinClientService;
            _litecoinClientService = litecoinClientService;

            HookEventHandlers();
        }

        /// <summary>
        /// Hook the event handlers with events of the implementations of CoinClientService
        /// </summary>
        private void HookEventHandlers()
        {
            _bitcoinClientService.DepositArrived -= this.OnDepositArrival;
            _bitcoinClientService.DepositConfirmed -= this.OnDepositConfirmed;
            _bitcoinClientService.DepositArrived += this.OnDepositArrival;
            _bitcoinClientService.DepositConfirmed += this.OnDepositConfirmed;

            _litecoinClientService.DepositArrived -= this.OnDepositArrival;
            _litecoinClientService.DepositConfirmed -= this.OnDepositConfirmed;
            _litecoinClientService.DepositArrived += this.OnDepositArrival;
            _litecoinClientService.DepositConfirmed += this.OnDepositConfirmed;
        }

        /// <summary>
        /// Selects the CoinClient Service to which to forward the event
        /// </summary>
        private ICoinClientService SelectCoinService(string currency)
        {
            switch (currency)
            {
                case CurrencyConstants.Btc:
                    return _bitcoinClientService;
                case CurrencyConstants.Ltc:
                    return _litecoinClientService;
            }
            return null;
        }

        /// <summary>
        /// Saveas a withdraw to database and starts teh timer after which the withrawal is submitted to CoinClientService
        /// </summary>
        /// <returns></returns>
        public bool CommitWithdraw(Withdraw withdraw)
        {
            if (withdraw != null)
            {
                Timer timer = new Timer(_withdrawSubmissioInterval);
                timer.Elapsed += OnTimerElapsed;
                timer.Enabled = true;
                _withdrawTimersDictionary.Add(timer, withdraw);
                return true;
            }
            throw new NullReferenceException(string.Format("Withdraw received is null"));
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
                return true;
            }
            throw new InstanceNotFoundException(string.Format("No withdraw found in memory for Withdraw ID: {0}",
                    withdrawId));
        }

        /// <summary>
        /// Sends request to generate a new address to the specific CoinClientService for the given currency
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public string GenerateNewAddress(string currency)
        {
            ICoinClientService coinService = SelectCoinService(currency);
            if (coinService != null)
            {
                // Select the corresponding coin client service and return a response after requesting
                return coinService.CreateNewAddress();
            }
            throw new InstanceNotFoundException(string.Format("No Client Service found for Currency: {0}", currency));
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
                    // Selecct the coin client service to which to send the withdraw
                    ICoinClientService coinClientService = SelectCoinService(withdraw.Currency.Name);
                    string transactionId = coinClientService.CommitWithdraw(
                                                             withdraw.BitcoinAddress.Value, withdraw.Amount);
                    // Set transaction Id recevied from the network
                    withdraw.SetTransactionId(transactionId);
                    /*
                    // Set status as confirmed
                    withdraw.StatusConfirmed();
                    // Save the withdraw
                    _fundsPersistenceRepository.SaveOrUpdate(withdraw);*/
                    _withdrawTimersDictionary.Remove(timer);
                    if (WithdrawExecuted != null)
                    {
                        WithdrawExecuted(withdraw);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the event raised by CoinClientServices when a deposit is confirmed
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="confirmations"></param>
        private void OnDepositConfirmed(string transactionId, int confirmations)
        {
            if (DepositConfirmed != null)
            {
                DepositConfirmed(transactionId, confirmations);
            }
        }

        /// <summary>
        /// Handles event raised in result when new transacitons are available. 
        /// Item1 = Address, Item2 = TransactionId, Item3 = Amount, Item4 = Category
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="newTransactions"></param>
        private void OnDepositArrival(string currency, List<Tuple<string, string, decimal, string>> newTransactions)
        {
            if (DepositArrived != null)
            {
                DepositArrived(currency, newTransactions);
            }
        }

        /// <summary>
        /// Interval after which Withdraw is submitted
        /// </summary>
        public double WithdrawSubmissionInterval
        {
            get { return _withdrawSubmissioInterval; }
        }
    }
}
