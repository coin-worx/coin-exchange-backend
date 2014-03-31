using CoinExchange.Funds.Domain.Model.Entities;

/*
 * Author: Waqas
 * Comany: Aurora Solutions
*/

namespace CoinExchange.Funds.Application.Commands
{
    /// <summary>
    /// A Service to serve requests considered private, like information regarding a user, portfolio, trades
    /// </summary>
    public class FundsCommands
    {
        /// <summary>
        /// Returns the Account Balance for all the currencies associated to a User
        /// </summary>
        /// <returns></returns>
        public AccountBalance[] GetAccountBalance(string assetName = null, string assetClass = null)
        {
            return new AccountBalance[]
                       {
                           new AccountBalance()
                               {
                                   AssetName = "XBT",
                                   Balance = 344220000
                               },
                           new AccountBalance()
                               {
                                    AssetName = "LTC",
                                    Balance = 85462500
                               },
                           new AccountBalance()
                               {
                                    AssetName = "XXRT",
                                    Balance = 12462500
                               },
                           new AccountBalance()
                               {
                                    AssetName = "USD",
                                    Balance = 978462500
                               },
                           new AccountBalance()
                               {
                                    AssetName = "EUR",
                                    Balance = 968462500
                               }
                       };
        }

        /// <summary>
        /// Returns the Trade Balance for all the currencies associated to a User
        /// </summary>
        /// <returns></returns>
        public TradeBalance GetTradeBalance(string assetName = null, string assetClass = null)
        {
            return new TradeBalance()
                       {
                           Asset = "XBT",
                           Balance = (decimal) 234515000.8745
                       };
        }

        /// <summary>
        /// Provides the Ledger's Info for all the ledgers
        /// </summary>
        /// <returns></returns>
        public LedgerInfo[] GetLedgers()
        {
            return new LedgerInfo[]
                       {
                           new LedgerInfo()
                               {
                                   RefId = 1423321,
                                   Asset = "XBT",
                                   AClass = "Currency",
                                   Amount = 345221,
                                   Balance = 653000000,
                                   Fee = 0.25,
                                   Time = "12-34-56"
                               }, 
                               new LedgerInfo()
                               {
                                   RefId = 1229821,
                                   Asset = "LTC",
                                   AClass = "Currency",
                                   Amount = 345221,
                                   Balance = 653000000,
                                   Fee = 0.25,
                                   Time = "02-32-54"
                               },
                               new LedgerInfo()
                               {
                                   RefId = 1900809,
                                   Asset = "USD",
                                   AClass = "Currency",
                                   Amount = 345221,
                                   Balance = 653000000,
                                   Fee = 0.25,
                                   Time = "09-12-07"
                               },
                               new LedgerInfo()
                               {
                                   RefId = 11099834,
                                   Asset = "EUR",
                                   AClass = "Currency",
                                   Amount = 345221,
                                   Balance = 653000000,
                                   Fee = 0.25,
                                   Time = "01-07-17"
                               },
                       };
        }
    }
}