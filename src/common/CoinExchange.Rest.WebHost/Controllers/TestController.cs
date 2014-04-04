using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace CoinExchange.Rest.WebHost.Controllers
{
    /// <summary>
    /// Controller just for testing purpose only, will be deleted...
    /// </summary>
    public class TestController : ApiController
    {
        [Authorize]
        [Route("test")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(("hello world"));

        }

        [Authorize]
        [Route("test")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] string param)
        {
            return Ok((param));

        }
        
    }
}