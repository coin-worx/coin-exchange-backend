using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace CoinExchange.Trades.Port.Adapter.Rest
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Enable attribute routing
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
