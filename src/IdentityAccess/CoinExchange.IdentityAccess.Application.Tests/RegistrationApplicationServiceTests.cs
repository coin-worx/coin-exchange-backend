using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.Registration;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    [TestFixture]
    public class RegistrationApplicationServiceTests
    {
        [Test]
        public void InjectionTest_TestsWhetherSpringInitiatesAsExpectedAndInitializesRegistrationService_FailsIfNotInitialized()
        {
            IApplicationContext applicationContext = ContextRegistry.GetContext();
            IRegistrationApplicationService registrationService =
                (IRegistrationApplicationService)applicationContext["RegistrationApplicationService"];
            Assert.IsNotNull(registrationService);
        }
    }
}
