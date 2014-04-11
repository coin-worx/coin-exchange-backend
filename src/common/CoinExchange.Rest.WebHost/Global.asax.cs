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
using Spring.Web.Mvc;

namespace CoinExchange.Rest.WebHost
{
    public class WebApiApplication : SpringMvcApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            //ControllerBuilder.Current.SetControllerFactory(new SpringControllerFactory());
           // RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configure(WebApiConfig.Register);
          //  LogManager.GetCurrentClassLogger().Info("Application started...");
            //IControllerFactory factory = new SpringControllerFactory();
            //ControllerBuilder.Current.SetControllerFactory(factory);
            //DependencyResolver.SetResolver(new SpringDependencyResolver(ContextRegistry.GetContext()));
        }
        protected override System.Web.Http.Dependencies.IDependencyResolver BuildWebApiDependencyResolver()
        {
            //get the 'default' resolver, populated from the 'main' config metadata
            var resolver = base.BuildWebApiDependencyResolver();

            //check if its castable to a SpringWebApiDependencyResolver
            var springResolver = resolver as SpringWebApiDependencyResolver;
            
            //return the fully-configured resolver
            return springResolver;
        }
    }
}
