using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Common.Services
{
    /// <summary>
    /// Service interface for getting information relevant to api key
    /// </summary>
    public interface IApiKeyInfoAccess
    {
        int GetUserIdFromApiKey(string apiKey);
    }
}
