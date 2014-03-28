using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CoinExchange.Trades.Domain.Model.VOs;
using CoinExchange.Trades.Port.Adapter.RestService;

namespace CoinExchange.Rest.WebHost.Controllers
{
    /// <summary>
    /// Controller for all market data requests
    /// </summary>
    public class MarketDataRequestController : ApiController
    {
        [HttpGet]
        [Route("marketData/tickerinfo")]
        public IHttpActionResult TickerInfo(string pair)
        {
            try
            {
                return Ok(new MarketDataService().GetTickerInfo(pair));
                
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

        [HttpGet]
        [Route("marketData/ohlcinfo")]
        public IHttpActionResult OhlcInfo(string pair,int interval=1,string since="")
        {
            try
            {
                return Ok(new MarketDataService().GetOhlcInfo(pair,interval,since));

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}