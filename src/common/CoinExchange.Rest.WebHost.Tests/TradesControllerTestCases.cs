using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Funds.Domain.Model.VOs;
using CoinExchange.Rest.WebHost.Controllers;
using CoinExchange.Trades.Domain.Model.Entities;
using NUnit.Framework;

namespace CoinExchange.Rest.WebHost.Tests
{
    /// <summary>
    /// Test cases for the Trades Controller
    /// </summary>
    class TradesControllerTestCases
    {
        /// <summary>
        /// Tests the functionality to fecth all the open orders
        /// </summary>
        [Test]
        public void GetOpenOrdersTestCase()
        {
            TradesRequestController fundsController = new TradesRequestController();
            IHttpActionResult httpActionResult = fundsController.OpenOrderList(new TraderId(2));

            // The result is and should be returned as IHttpActionResult, which contains content as well as response codes for
            // Http response messages sent to the client.  If it is not null, menas the request was successful
            Assert.IsNotNull(httpActionResult);
            Assert.AreEqual(httpActionResult.GetType(), typeof(OkNegotiatedContentResult<List<Order>>));
            OkNegotiatedContentResult<List<Order>> okResponseMessage = (OkNegotiatedContentResult<List<Order>>)httpActionResult;

            // If the response message contains content and its count is greater than 0, our test is successful
            Assert.IsNotNull(okResponseMessage.Content);
            Assert.GreaterOrEqual(okResponseMessage.Content.Count(), 1, "Count of the contents in the OK response message");
        }
    }
}
