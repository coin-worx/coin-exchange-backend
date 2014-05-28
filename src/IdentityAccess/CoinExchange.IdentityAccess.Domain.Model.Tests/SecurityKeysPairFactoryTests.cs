using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Domain.Model.Tests
{
    [TestFixture]
    public class SecurityKeysPairFactoryTests:ISecurityKeysGenerationService
    {
        [Test]
        [Category("Unit")]
        public void CreateSystemGeneratedSecurityKeysPair_IfUserNameIsProvided_TheSecurityPairShouldHaveBeenCreated()
        {
            SecurityKeysPair pair = SecurityKeysPairFactory.SystemGeneratedSecurityKeyPair("user1",this);
            Assert.NotNull(pair);
            Assert.AreEqual(pair.UserName, "user1");
            Assert.AreEqual(pair.SystemGenerated, true);
            Assert.IsNotNullOrEmpty(pair.ApiKey);
            Assert.IsNotNullOrEmpty(pair.SecretKey);
            Assert.IsNotNullOrEmpty(pair.KeyDescription);
            Assert.IsNotNullOrEmpty(pair.CreationDateTime.ToString());
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateSystemGeneratedSecurityKeysPair_IfUserNameIsEmpty_ArgumentNullExceptionShouldReceived()
        {
            SecurityKeysPair pair = SecurityKeysPairFactory.SystemGeneratedSecurityKeyPair("",this);
        }

        [Test]
        [Category("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateSystemGeneratedSecurityKeysPair_IfUserNameIsNull_ArgumentNullExceptionShouldReceived()
        {
            SecurityKeysPair pair = SecurityKeysPairFactory.SystemGeneratedSecurityKeyPair(null,this);
        }

        /// <summary>
        /// Stub Interface implementation
        /// </summary>
        /// <returns></returns>
        public Tuple<string, string> GenerateNewSecurityKeys()
        {
            return new Tuple<string, string>("api","secret");
        }
    }
}
