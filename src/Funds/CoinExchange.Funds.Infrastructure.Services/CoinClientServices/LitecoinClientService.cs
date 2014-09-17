using System;
using System.Collections.Generic;
using System.Configuration;
using BitcoinLib.Services.Coins.Base;
using BitcoinLib.Services.Coins.Litecoin;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Infrastructure.Services.CoinClientServices
{
    /// <summary>
    /// Litecoin Client Service
    /// </summary>
    public class LitecoinClientService : ICoinClientService
    {
        private ICoinService _litecoinService = null;
        public event Action<string, List<Tuple<string, string, decimal, string>>> DepositArrived;
        public event Action<string, int> DepositConfirmed;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LitecoinClientService()
        {
            bool useBitcoinTestNet = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("LtcUseTestNet"));
            _litecoinService = new LitecoinService(useTestnet: useBitcoinTestNet);
        }

        /// <summary>
        /// Create New Address using the daemon
        /// </summary>
        /// <returns></returns>
        public string CreateNewAddress()
        {
            return _litecoinService.GetNewAddress();
        }

        public string CommitWithdraw(string bitcoinAddress, decimal amount)
        {
            throw new NotImplementedException();
        }

        public void PopulateCurrencies()
        {
            throw new NotImplementedException();
        }

        public void PopulateServices()
        {
            throw new NotImplementedException();
        }

        public decimal CheckBalance(string currency)
        {
            throw new NotImplementedException();
        }

        public double PollingInterval { get; private set; }
    }
}
