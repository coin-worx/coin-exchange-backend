using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CoinExchange.Rest.WebHost.App_Start;
using Common.Logging;
using Spring.Context.Support;

namespace CoinExchange.Rest.WebHost
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            LogManager.GetCurrentClassLogger().Info("Application started...");

            DependencyResolver.SetResolver(new SpringDependencyResolver(ContextRegistry.GetContext()));
        }
    }
}
