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
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Commands;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Infrastructure.Persistence.Projection;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.IntegrationTests
{
    [TestFixture]
    public class SecurityKeysControllerTests
    {
        private IApplicationContext _applicationContext;
        private DatabaseUtility _databaseUtility;
        [SetUp]
        public void Setup()
        {
            _applicationContext = ContextRegistry.GetContext();
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
        }

        [TearDown]
        public void TearDown()
        {
            ContextRegistry.Clear();
            _databaseUtility.Create();
        }

        [Test]
        [Category("Integration")]
        public void CreateUsergeneratedSystemKey_ProvideAllParameters_TheKeysShouldBeReturned()
        {
            UserValidationEssentials essentials = AccessControlUtility.RegisterAndLogin("user", "user@user.com", "123", _applicationContext);
            SecurityKeyPairController securityKeyPairController =
                _applicationContext["SecurityKeyPairController"] as SecurityKeyPairController;
            IPermissionRepository permissionRepository = _applicationContext["PermissionRespository"] as IPermissionRepository;
            IList<Permission> permissions = permissionRepository.GetAllPermissions();
            List<SecurityKeyPermissionsRepresentation> securityKeyPermissions=new List<SecurityKeyPermissionsRepresentation>();
            for (int i = 0; i < permissions.Count; i++)
            {
                securityKeyPermissions.Add(new SecurityKeyPermissionsRepresentation(true,permissions[i]));
            }
            securityKeyPairController.Request = new HttpRequestMessage(HttpMethod.Post, "");
            securityKeyPairController.Request.Headers.Add("Auth", essentials.ApiKey.Value);
            CreateUserGeneratedSecurityKeyPair command=new CreateUserGeneratedSecurityKeyPair(securityKeyPermissions.ToArray(),"","","",false,false,false,"#1");
            IHttpActionResult httpActionResult=securityKeyPairController.CreateSecurityKey(command);
            OkNegotiatedContentResult<Tuple<string, string>> result = (OkNegotiatedContentResult<Tuple<string,string>>)httpActionResult;
            Assert.IsNotNullOrEmpty(result.Content.Item1);
            Assert.IsNotNullOrEmpty(result.Content.Item2);

            httpActionResult = securityKeyPairController.GetUserSecurityKeys();
            OkNegotiatedContentResult<object> result1 = (OkNegotiatedContentResult<object>)httpActionResult;
            IList<SecurityKeyPairList> pairs = result1.Content as IList<SecurityKeyPairList>;
            Assert.AreEqual(pairs.Count,1);
            Assert.AreEqual(pairs[0].KeyDescription, "#1");
            Assert.IsNull(pairs[0].ExpirationDate);

            httpActionResult = securityKeyPairController.GetSecurityKeyDetail("#1");
            OkNegotiatedContentResult<SecurityKeyRepresentation> securityKey = (OkNegotiatedContentResult<SecurityKeyRepresentation>)httpActionResult;
            Assert.AreEqual(securityKey.Content.KeyDescritpion,"#1");
            Assert.AreEqual(securityKey.Content.EnableEndDate, false);
            Assert.AreEqual(securityKey.Content.EnableExpirationDate,false); 
            Assert.AreEqual(securityKey.Content.EnableStartDate, false);
        }
    }
}
