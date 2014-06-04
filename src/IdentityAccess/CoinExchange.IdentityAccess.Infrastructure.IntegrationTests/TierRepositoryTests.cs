using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    [TestFixture]
    public class TierRepositoryTests:AbstractConfiguration
    {
        private ITierRepository _tierRepository;

        //property will be injected base on type
        public ITierRepository TierRepository
        {
            set { _tierRepository = value; }
        }

        private DatabaseUtility _databaseUtility;

        [SetUp]
        public void Setup()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
        }

        [Test]
        [Category("Integration")]
        public void GetAllTierLevel_IfMasterDataIsPresentInDatabase_ThereShooldBe5TierLevels()
        {
            IList<Tier> geTiers = _tierRepository.GetAllTierLevels();
            Assert.NotNull(geTiers);
            Assert.AreEqual(5,geTiers.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(geTiers[i].TierLevel,"Tier "+i);
            }
        }
    }
}
