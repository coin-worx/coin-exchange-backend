using System.Collections.Generic;
using CoinExchange.Funds.Domain.Model.Entities;
/*
 * Author: Waqas
 * Comany: Aurora Solutions
*/
using CoinExchange.Funds.Domain.Model.VOs;

namespace CoinExchange.Funds.Application.Funds
{
    // ToDo: Need to further divide this service into granular peices for Funds bounded contexts. For now, we leave it as it is 
    /// <summary>
    /// A Service to serve requests considered private, like information regarding a user, portfolio, trades
    /// </summary>
    public class FundsApplicationService
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
                                   AssetClass = "Currency",
                                   Amount = 345221,
                                   Balance = 653000000,
                                   Fee = 0.25,
                                   Time = "12-34-56"
                               }, 
                               new LedgerInfo()
                               {
                                   RefId = 1229821,
                                   Asset = "LTC",
                                   AssetClass = "Currency",
                                   Amount = 345221,
                                   Balance = 653000000,
                                   Fee = 0.25,
                                   Time = "02-32-54"
                               },
                               new LedgerInfo()
                               {
                                   RefId = 1900809,
                                   Asset = "USD",
                                   AssetClass = "Currency",
                                   Amount = 345221,
                                   Balance = 653000000,
                                   Fee = 0.25,
                                   Time = "09-12-07"
                               },
                               new LedgerInfo()
                               {
                                   RefId = 11099834,
                                   Asset = "EUR",
                                   AssetClass = "Currency",
                                   Amount = 345221,
                                   Balance = 653000000,
                                   Fee = 0.25,
                                   Time = "01-07-17"
                               },
                       };
        }

        /// <summary>
        /// Returns list of tradeable assets
        /// </summary>
        /// <param name="info"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        public List<TradeableAsset> TradeabelAssetPair(string info, string pair)
        {
            AssetFee fee = new AssetFee(0.002m, 100);
            List<AssetFee> fees = new List<AssetFee>();
            for (int i = 0; i < 10; i++)
            {
                fees.Add(fee);
            }
            TradeableAsset asset = new TradeableAsset(10, 10, "USD", fees, null, 10, 2, 2, "Unit", "XXDG", "currency", "currency", "LTCXDG");
            List<TradeableAsset> assets = new List<TradeableAsset>();
            for (int i = 0; i < 10; i++)
            {
                assets.Add(asset);
            }
            return assets;
        }
    }
}