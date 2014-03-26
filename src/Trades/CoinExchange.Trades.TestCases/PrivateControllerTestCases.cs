using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using CoinExchange.Trades.Port.Adapter.Rest.Controllers;
using CoinExchange.Trades.Port.Adapter.Rest.Models;
using NUnit.Framework;
using System.Web.Http;

/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

namespace CoinExchange.Trades.TestCases
{
    /// <summary>
    /// Contians test cases for 'PrivateController'
    /// </summary>
    [TestFixture]
    class PrivateControllerTestCases
    {
        /// <summary>
        /// Gets the Trade balances for a list o Currencies associated to a user
        /// </summary>
        [Test]
        public void GetTradeBalanceTestCase()
        {
            PrivateController privateController = new PrivateController();
            IHttpActionResult httpActionResult = privateController.GetBalance();

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
            PrivateController privateController = new PrivateController();
            IHttpActionResult httpActionResult = privateController.GetLedger();

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
