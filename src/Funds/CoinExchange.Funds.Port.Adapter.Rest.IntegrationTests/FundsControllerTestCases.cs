/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Funds.Domain.Model;
using CoinExchange.Funds.Domain.Model.Entities;
using CoinExchange.Funds.Domain.Model.VOs;
using CoinExchange.Funds.Port.Adapter.Rest.Controllers;
using NUnit.Framework;
using System.Linq;

namespace CoinExchange.Funds.Port.Adapter.Rest.IntegrationTests
{
    /// <summary>
    /// Contians test cases for 'PrivateController'
    /// </summary>
    [TestFixture]
    class FundsControllerTestCases
    {
        /// <summary>
        /// Gets the Trade balances for a list o Currencies associated to a user
        /// </summary>
        [Test]
        public void GetTradeBalanceTestCase()
        {
            FundsController fundsController = new FundsController();
            IHttpActionResult httpActionResult = fundsController.AccountBalance(new TraderId(1));

            // The result is and should be returned as IHttpActionResult, which contains content as well as response codes for
            // Http response messages sent to the client.  If it is not null, menas the request was successful
            Assert.IsNotNull(httpActionResult);
            Assert.AreEqual(httpActionResult.GetType(), typeof (OkNegotiatedContentResult<AccountBalance[]>));
            OkNegotiatedContentResult<AccountBalance[]> okResponseMessage = (OkNegotiatedContentResult<AccountBalance[]>)httpActionResult;
            
            // If the response message contains content and its count is greater than 0, our test is successful
            Assert.IsNotNull(okResponseMessage.Content);
            Assert.GreaterOrEqual(okResponseMessage.Content.Count(), 1, "Count of the contents in the OK response message");
        }

        /// <summary>
        /// Tests the method that gets the Ledgers Info for all the Ledgers
        /// </summary>
        [Test]
        public void GetLedgersInfoTestCase()
        {
            FundsController fundsController = new FundsController();
            IHttpActionResult httpActionResult = fundsController.LedgerInfo(new TraderId(2), new LedgerInfoRequest()
                                                                                                 {
                                                                                                     AssetClass = "Currency",
                                                                                                     Asset = "XBT"
                                                                                                 });

            // The result is and should be returned as IHttpActionResult, which contains content as well as response codes for
            // Http response messages sent to the client.  If it is not null, menas the request was successful
            Assert.IsNotNull(httpActionResult);
            Assert.AreEqual(httpActionResult.GetType(), typeof(OkNegotiatedContentResult<LedgerInfo[]>));
            OkNegotiatedContentResult<LedgerInfo[]> okResponseMessage = (OkNegotiatedContentResult<LedgerInfo[]>)httpActionResult;

            // If the response message contains content and its count is greater than 0, our test is successful
            Assert.IsNotNull(okResponseMessage.Content);
            Assert.GreaterOrEqual(okResponseMessage.Content.Count(), 1, "Count of the contents in the OK response message");
        }

        /// <summary>
        /// Tests the method that fetches the ledgers
        /// </summary>
        [Test]
        public void FetchLedgersTestCase()
        {
            FundsController fundsController = new FundsController();
            IHttpActionResult httpActionResult = fundsController.FetchLedgers(new TraderId(2), "1");

            // The result is and should be returned as IHttpActionResult, which contains content as well as response codes for
            // Http response messages sent to the client.  If it is not null, menas the request was successful
            Assert.IsNotNull(httpActionResult);
            Assert.AreEqual(httpActionResult.GetType(), typeof(OkNegotiatedContentResult<LedgerInfo[]>));
            OkNegotiatedContentResult<LedgerInfo[]> okResponseMessage = (OkNegotiatedContentResult<LedgerInfo[]>)httpActionResult;

            // If the response message contains content and its count is greater than 0, our test is successful
            Assert.IsNotNull(okResponseMessage.Content);
            Assert.GreaterOrEqual(okResponseMessage.Content.Count(), 1, "Count of the contents in the OK response message");
        }
    }
}
