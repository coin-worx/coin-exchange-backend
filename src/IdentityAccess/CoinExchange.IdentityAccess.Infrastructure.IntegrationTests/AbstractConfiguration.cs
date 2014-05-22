using NUnit.Framework;
using Spring.Testing.NUnit;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    /// <summary>
    /// http://www.springframework.net/docs/1.3.0/reference/html/testing.html#testing-ctx-management
    /// Context Management & Caching
    /// http://www.springframework.net/docs/1.3.0/reference/html/testing.html#testing-tx
    /// </summary>
    [TestFixture]
    public abstract class AbstractConfiguration : AbstractTransactionalDbProviderSpringContextTests
    {
        protected override string[] ConfigLocations
        {
            get
            {
                return new[]
                    {
                        "assembly://CoinExchange.IdentityAccess.Infrastructure.Persistence/CoinExchange.IdentityAccess.Infrastructure.Persistence/IdentityAccessSpringConfig.xml",
                    };
            }
        }
    }
}
