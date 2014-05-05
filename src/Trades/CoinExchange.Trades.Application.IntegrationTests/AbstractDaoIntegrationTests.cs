using NUnit.Framework;
using Spring.Testing.NUnit;

namespace CoinExchange.Trades.Application.IntegrationTests
{
    /// <summary>
    /// http://www.springframework.net/docs/1.3.0/reference/html/testing.html#testing-ctx-management
    /// Context Management & Caching
    /// http://www.springframework.net/docs/1.3.0/reference/html/testing.html#testing-tx
    /// </summary>
    [TestFixture]
    public abstract class AbstractDaoIntegrationTests : AbstractTransactionalDbProviderSpringContextTests
    {
        protected override string[] ConfigLocations
        {
            get
            {
                return new[]
                    {
                        "assembly://CoinExchange.Trades.ReadModel.Persistence/CoinExchange.Trades.ReadModel.Persistence/SpringConfig.xml",
                        "assembly://CoinExchange.Trades.Infrastructure.Services/CoinExchange.Trades.Infrastructure.Services.Config/StubTradeIdGenerator.xml"
                    };
            }
        }
        
    }
}
