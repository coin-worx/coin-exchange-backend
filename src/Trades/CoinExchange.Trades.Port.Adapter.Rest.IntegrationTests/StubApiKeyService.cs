using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Services;

namespace CoinExchange.Trades.Port.Adapter.Rest.IntegrationTests
{
    //stub implementation of api key service
    public class StubApiKeyService:IApiKeyInfoAccess
    {
        public int GetUserIdFromApiKey(string apiKey)
        {
            return int.Parse(Constants.GetTraderId(apiKey));
        }
    }
}
