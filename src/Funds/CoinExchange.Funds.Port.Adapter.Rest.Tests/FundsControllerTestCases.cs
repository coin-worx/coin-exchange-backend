/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Funds.Domain.Model;
using CoinExchange.Funds.Port.Adapter.Rest.Controllers;
using NUnit.Framework;
using System.Linq;

namespace CoinExchange.Funds.Port.Adapter.Rest.Tests
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
            FundsController privateController = new FundsController();
            IHttpActionResult httpActionResult = privateController.GetBalance(1);

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
            FundsController privateController = new FundsController();
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
