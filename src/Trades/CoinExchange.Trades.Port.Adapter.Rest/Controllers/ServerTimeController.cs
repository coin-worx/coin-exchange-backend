using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CoinExchange.Trades.Port.Adapter.Rest.Controllers
{
    /// <summary>
    /// Responsible for server date time requests
    /// </summary>
    public class ServerTimeController : ApiController
    {
        [Route("server/time")]
        public DateTime Get()
        {
            return DateTime.Now;
        }
    }
}