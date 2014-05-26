using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    [TestFixture]
    public class SecurityKeysPairRepositoryTests:AbstractConfiguration
    {
        private IIdentityAccessPersistenceRepository _persistenceRepository;
        private ISecurityKeysRepository _securityKeysPairRepository;
        private IPermissionRepository _permissionRepository;

        //properties will be injected based on type
        public IIdentityAccessPersistenceRepository PersistenceRepository
        {
            set { _persistenceRepository = value; }
        }

        public ISecurityKeysRepository DigitalSignatureInfoRepository
        {
            set { _securityKeysPairRepository = value; }
        }

        public IPermissionRepository PermissionRepository
        {
            set { _permissionRepository = value; }
        }

        [Test]
        [Category("Integration")]
        public void CreateSecurityKeyPair_PersistAndReadFromDatabaseByDescriptionKey_SavedAndReadInfoShouldBeSame()
        {
            SecurityKeysPair digitalSignatureInfo=new SecurityKeysPair("1", new ApiKey("123456"), new SecretKey("secretkey"),"user1",DateTime.Today.AddDays(1),DateTime.Today.AddDays(-20),DateTime.Today,DateTime.Now,true,null);
            _persistenceRepository.SaveUpdate(digitalSignatureInfo);
            var readInfo = _securityKeysPairRepository.GetByKeyDescription("1");
            Assert.NotNull(readInfo);
            Assert.AreEqual(readInfo.KeyDescription,"1");
            Assert.AreEqual(readInfo.ApiKey.Value, "123456");
            Assert.AreEqual(readInfo.SecretKey.Value, "secretkey");
            Assert.AreEqual(readInfo.UserName, digitalSignatureInfo.UserName);
            Assert.AreEqual(readInfo.SystemGenerated, digitalSignatureInfo.SystemGenerated);
            Assert.AreEqual(readInfo.ExpirationDate, digitalSignatureInfo.ExpirationDate);
            Assert.AreEqual(readInfo.StartDate, digitalSignatureInfo.StartDate);
            Assert.AreEqual(readInfo.EndDate, digitalSignatureInfo.EndDate);
        }
        [Test]
        [Category("Integration")]
        public void CreateSecurityKeyPair_PersistAndReadFromDatabaseByApiKey_SavedAndReadInfoShouldBeSame()
        {
            SecurityKeysPair digitalSignatureInfo = new SecurityKeysPair("1", new ApiKey("123456"), new SecretKey("secretkey"), "user1", DateTime.Today.AddDays(1), DateTime.Today.AddDays(-20), DateTime.Today, DateTime.Now, true,null);
            _persistenceRepository.SaveUpdate(digitalSignatureInfo);
            var readInfo = _securityKeysPairRepository.GetByApiKey("123456");
            Assert.NotNull(readInfo);
            Assert.AreEqual(readInfo.KeyDescription, "1");
            Assert.AreEqual(readInfo.ApiKey.Value, "123456");
            Assert.AreEqual(readInfo.SecretKey.Value, "secretkey");
            Assert.AreEqual(readInfo.UserName, digitalSignatureInfo.UserName);
            Assert.AreEqual(readInfo.SystemGenerated, digitalSignatureInfo.SystemGenerated);
            Assert.AreEqual(readInfo.ExpirationDate, digitalSignatureInfo.ExpirationDate);
            Assert.AreEqual(readInfo.StartDate, digitalSignatureInfo.StartDate);
            Assert.AreEqual(readInfo.EndDate, digitalSignatureInfo.EndDate);
        }

        [Test]
        [Category("Integration")]
        public void CreateSecurityKeyPair_AssignPermissionAndPersist_ItShouldGetSavedInTheDatabase()
        {
            IList<Permission> permissions = _permissionRepository.GetAllPermissions();

            IList<SecurityKeysPermission> securityKeysPermissions=new List<SecurityKeysPermission>();
            for (int i = 0; i < 7; i++)
            {
                SecurityKeysPermission permission = new SecurityKeysPermission("1", permissions[i], true);
                securityKeysPermissions.Add(permission);
            }
            SecurityKeysPair securityKeys = new SecurityKeysPair("1", new ApiKey("123456"), new SecretKey("secretkey"), "user1", DateTime.Today.AddDays(1), DateTime.Today.AddDays(-20), DateTime.Today, DateTime.Now, true,securityKeysPermissions);
            _persistenceRepository.SaveUpdate(securityKeys);
            var readInfo = _securityKeysPairRepository.GetByApiKey("123456");
            Assert.NotNull(readInfo);
            Assert.AreEqual(readInfo.KeyDescription, "1");
            Assert.AreEqual(readInfo.ApiKey.Value, "123456");
            Assert.AreEqual(readInfo.SecretKey.Value, "secretkey");
            Assert.AreEqual(readInfo.UserName, securityKeys.UserName);
            Assert.AreEqual(readInfo.SystemGenerated, securityKeys.SystemGenerated);
            Assert.AreEqual(readInfo.ExpirationDate, securityKeys.ExpirationDate);
            Assert.AreEqual(readInfo.StartDate, securityKeys.StartDate);
            Assert.AreEqual(readInfo.EndDate, securityKeys.EndDate);
            ValidatePermissions(readInfo,securityKeysPermissions.ToArray());
        }

        [Test]
        [Category("Integration")]
        public void CreateSecurityKeyPair_UpdatePermission_ItShouldGetUpdatedInTheDatabase()
        {
            IList<Permission> permissions = _permissionRepository.GetAllPermissions();

            IList<SecurityKeysPermission> securityKeysPermissions = new List<SecurityKeysPermission>();
            for (int i = 0; i < 7; i++)
            {
                SecurityKeysPermission permission = new SecurityKeysPermission("1", permissions[i], true);
                securityKeysPermissions.Add(permission);
            }
            SecurityKeysPair securityKeys = new SecurityKeysPair("1", new ApiKey("123456"), new SecretKey("secretkey"), "user1", DateTime.Today.AddDays(1), DateTime.Today.AddDays(-20), DateTime.Today, DateTime.Now, true, securityKeysPermissions);
            _persistenceRepository.SaveUpdate(securityKeys);
            //update permissions
            securityKeysPermissions[2].IsAllowed = false;
            securityKeysPermissions[4].IsAllowed = false;
            securityKeysPermissions[6].IsAllowed = false;
            securityKeys.UpdatePermissions(securityKeysPermissions.ToArray());
            //save them again
            _persistenceRepository.SaveUpdate(securityKeys);
            //read security key pair
            var readInfo = _securityKeysPairRepository.GetByApiKey("123456");
            Assert.NotNull(readInfo);
            Assert.AreEqual(readInfo.KeyDescription, "1");
            Assert.AreEqual(readInfo.ApiKey.Value, "123456");
            Assert.AreEqual(readInfo.SecretKey.Value, "secretkey");
            Assert.AreEqual(readInfo.UserName, securityKeys.UserName);
            Assert.AreEqual(readInfo.SystemGenerated, securityKeys.SystemGenerated);
            Assert.AreEqual(readInfo.ExpirationDate, securityKeys.ExpirationDate);
            Assert.AreEqual(readInfo.StartDate, securityKeys.StartDate);
            Assert.AreEqual(readInfo.EndDate, securityKeys.EndDate);
            ValidatePermissions(readInfo,securityKeysPermissions.ToArray());
        }

        /// <summary>
        /// Validate permissions
        /// </summary>
        /// <param name="keysPair"></param>
        /// <param name="permissions"></param>
        private void ValidatePermissions(SecurityKeysPair keysPair,SecurityKeysPermission[] permissions)
        {
            for (int i = 0; i < permissions.Length; i++)
            {
                Assert.AreEqual(permissions[i].IsAllowed,
                    keysPair.ValidatePermission(permissions[i].Permission.PermissionId));
            }
        }

    }
}
