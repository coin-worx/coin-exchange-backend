using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using CoinExchange.Rest.WebHost.App_Start;
using Common.Logging;


namespace CoinExchange.Rest.WebHost
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
          GlobalConfiguration.Configure(WebApiConfig.Register);
          LogManager.GetCurrentClassLogger().Info("Application started...");
        }
    }
}
