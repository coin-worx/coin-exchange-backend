using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Common.Utility
{
    /// <summary>
    /// class for getting required parameter from http header
    /// </summary>
    public static class HeaderParamUtility
    {
        public static string GetApikey(HttpRequestMessage requestMessage)
        {
            var headers = requestMessage.Headers;
            string apikey = "";
            IEnumerable<string> headerParams;
            if (headers.TryGetValues("Auth", out headerParams))
            {
                string[] auth = headerParams.ToList()[0].Split(',');
                apikey = auth[0];
            }
            return apikey;
        }
    }
}
