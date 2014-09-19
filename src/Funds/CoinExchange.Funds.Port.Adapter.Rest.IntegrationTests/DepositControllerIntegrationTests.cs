using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Common.Tests;
using CoinExchange.Funds.Application.DepositServices.Representations;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Infrastructure.Persistence.NHibernate.NHibernate;
using CoinExchange.Funds.Port.Adapter.Rest.DTOs.Deposit;
using CoinExchange.Funds.Port.Adapter.Rest.Resources;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Funds.Port.Adapter.Rest.IntegrationTests
{
    [TestFixture]
    class DepositControllerIntegrationTests
    {
        private DatabaseUtility _databaseUtility;

        [SetUp]
        public void Setup()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
        }

        [TearDown]
        public void Teardown()
        {
            _databaseUtility.Create();
        }

        [Test]
        public void DepositControllerInitializationTest_ChecksIfTheControllerInitializesAsExpected_VerifiesThroughInstance()
        {
            DepositController depositController = (DepositController)ContextRegistry.GetContext()["DepositController"];
            Assert.IsNotNull(depositController);
        }

        [Test]
        [Category("Integration")]
        public void DepositAddressTest_NewAddressMustBeGenerated_VerifiesThroughDatabaseQueryAndReturnValue()
        {
            DepositController depositController = (DepositController)ContextRegistry.GetContext()["DepositController"];
            IDepositAddressRepository depositAddressRepository = (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
            Assert.IsNotNull(depositController);

            string apiKey = "apiKey";
            depositController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            depositController.Request.Headers.Add("Auth", apiKey);
            IHttpActionResult httpActionResult = depositController.CreateDepositAddress(new GenerateAddressParams(1, "LTC"));
            OkNegotiatedContentResult<DepositAddressRepresentation> response = 
                (OkNegotiatedContentResult<DepositAddressRepresentation>) httpActionResult;
            Assert.IsNotNull(response);
            Assert.IsTrue(!string.IsNullOrEmpty(response.Content.Address));
            Assert.AreEqual(AddressStatus.New.ToString(), response.Content.Status);

            DepositAddress depositAddress = depositAddressRepository.GetDepositAddressByAddress(new BitcoinAddress(response.Content.Address));
            Assert.IsNotNull(depositAddress);
            Assert.AreEqual(AddressStatus.New, depositAddress.Status);
        }

        [Test]
        [Category("Integration")]
        public void DepositTest_NewAddressMustBeGenerated_VerifiesThroughDatabaseQueryAndReturnValue()
        {
            DepositController depositController = (DepositController)ContextRegistry.GetContext()["DepositController"];
            IDepositAddressRepository depositAddressRepository = (IDepositAddressRepository)ContextRegistry.GetContext()["DepositAddressRepository"];
            Assert.IsNotNull(depositController);

            string apiKey = "apiKey";
            depositController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            depositController.Request.Headers.Add("Auth", apiKey);
            IHttpActionResult httpActionResult = depositController.CreateDepositAddress(new GenerateAddressParams(1, "LTC"));
            OkNegotiatedContentResult<DepositAddressRepresentation> response =
                (OkNegotiatedContentResult<DepositAddressRepresentation>)httpActionResult;
            Assert.IsNotNull(response);
            Assert.IsTrue(!string.IsNullOrEmpty(response.Content.Address));
            Assert.AreEqual(AddressStatus.New.ToString(), response.Content.Status);

            DepositAddress depositAddress = depositAddressRepository.GetDepositAddressByAddress(new BitcoinAddress(response.Content.Address));
            Assert.IsNotNull(depositAddress);
            Assert.AreEqual(AddressStatus.New, depositAddress.Status);

            httpActionResult = depositController.GetDepositAddresses(new GetDepositAddressesParams("LTC"));
            OkNegotiatedContentResult<IList<DepositAddressRepresentation>> depositAddresses =
                (OkNegotiatedContentResult<IList<DepositAddressRepresentation>>) httpActionResult;
            Assert.IsNotNull(depositAddresses.Content);
            Assert.Greater(depositAddresses.Content.Count, 0);
            Assert.AreEqual(response.Content.Address, depositAddresses.Content[0].Address);
            Assert.AreEqual(response.Content.Status, depositAddresses.Content[0].Status);
        }
    }
}
