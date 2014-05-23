using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    [TestFixture]
    public class DigitalSignatureInfoRepositoryTests:AbstractConfiguration
    {
        private IPersistenceRepository _persistenceRepository;
        private IDigitalSignatureInfoRepository _digitalSignatureInfoRepository;

        //properties will be injected based on type
        public IPersistenceRepository PersistenceRepository
        {
            set { _persistenceRepository = value; }
        }

        public IDigitalSignatureInfoRepository DigitalSignatureInfoRepository
        {
            set { _digitalSignatureInfoRepository = value; }
        }

        [Test]
        [Category("Integration")]
        public void CreateDigitalSignatureInfo_PersistAndReadFromDatabaseByDescriptionKey_SavedAndReadInfoShouldBeSame()
        {
            DigitalSignature keys=new DigitalSignature("123456","secretkey");
            DigitalSignatureInfo digitalSignatureInfo=new DigitalSignatureInfo("1",keys,"user1",DateTime.Today.AddDays(1),DateTime.Today.AddDays(-20),DateTime.Today,DateTime.Now,true);
            _persistenceRepository.SaveUpdate(digitalSignatureInfo);
            var readInfo = _digitalSignatureInfoRepository.GetByKeyDescription("1");
            Assert.NotNull(readInfo);
            Assert.AreEqual(readInfo.KeyDescription,"1");
            Assert.AreEqual(readInfo.SecurityKeys, keys);
            Assert.AreEqual(readInfo.UserName, digitalSignatureInfo.UserName);
            Assert.AreEqual(readInfo.SystemGenerated, digitalSignatureInfo.SystemGenerated);
            Assert.AreEqual(readInfo.ExpirationDate, digitalSignatureInfo.ExpirationDate);
            Assert.AreEqual(readInfo.StartDate, digitalSignatureInfo.StartDate);
            Assert.AreEqual(readInfo.EndDate, digitalSignatureInfo.EndDate);
        }
        [Test]
        [Category("Integration")]
        public void CreateDigitalSignatureInfo_PersistAndReadFromDatabaseByApiKey_SavedAndReadInfoShouldBeSame()
        {
            DigitalSignature keys = new DigitalSignature("123456", "secretkey");
            DigitalSignatureInfo digitalSignatureInfo = new DigitalSignatureInfo("1", keys, "user1", DateTime.Today.AddDays(1), DateTime.Today.AddDays(-20), DateTime.Today, DateTime.Now, true);
            _persistenceRepository.SaveUpdate(digitalSignatureInfo);
            var readInfo = _digitalSignatureInfoRepository.GetByApiKey("123456");
            Assert.NotNull(readInfo);
            Assert.AreEqual(readInfo.KeyDescription, "1");
            Assert.AreEqual(readInfo.SecurityKeys, keys);
            Assert.AreEqual(readInfo.UserName, digitalSignatureInfo.UserName);
            Assert.AreEqual(readInfo.SystemGenerated, digitalSignatureInfo.SystemGenerated);
            Assert.AreEqual(readInfo.ExpirationDate, digitalSignatureInfo.ExpirationDate);
            Assert.AreEqual(readInfo.StartDate, digitalSignatureInfo.StartDate);
            Assert.AreEqual(readInfo.EndDate, digitalSignatureInfo.EndDate);
        }

    }
}
