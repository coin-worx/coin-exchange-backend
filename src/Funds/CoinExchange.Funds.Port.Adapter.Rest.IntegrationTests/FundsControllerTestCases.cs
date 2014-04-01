using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.Entities;
using CoinExchange.Funds.Domain.Model.VOs;
using CoinExchange.Funds.Port.Adapter.Rest.Controllers;
using NUnit.Framework;

namespace CoinExchange.Funds.Port.Adapter.Rest.IntegrationTests
{
    [TestFixture]
    class FundsControllerTestCases
    {
        /// <summary>
        /// Gets the Trade balances for a list o Currencies associated to a user
        /// </summary>
        [Test]
        public void GetTradeBalanceTestCase()
        {
            FundsResource fundsController = new FundsResource();
            IHttpActionResult httpActionResult = fundsController.AccountBalance(new TraderId(1));

            // The result is and should be returned as IHttpActionResult, which contains content as well as response codes for
            // Http response messages sent to the client.  If it is not null, menas the request was successful
            Assert.IsNotNull(httpActionResult);
            Assert.AreEqual(httpActionResult.GetType(), typeof(OkNegotiatedContentResult<AccountBalance[]>));
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
            FundsResource fundsController = new FundsResource();
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
            FundsResource fundsController = new FundsResource();
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
