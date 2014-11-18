using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using CoinExchange.Rest.WebHost;
using CoinExchange.Rest.WebHost.App_Start;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace CoinExchange.Rest.WebHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            Swashbuckle.Bootstrapper.Init(config);
            app.Use(config);
        }
    }
}