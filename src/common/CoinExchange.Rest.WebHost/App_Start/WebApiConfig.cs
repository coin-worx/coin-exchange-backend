using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using CoinExchange.IdentityAccess.Application;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices;
using Common.Logging;
using Spring.Context.Support;

namespace CoinExchange.Rest.WebHost.App_Start
{
    public static class WebApiConfig
    {
        //get current logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //return Json format
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/json");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            config.DependencyResolver = new SpringDependencyResolver(ContextRegistry.GetContext());
            Thread.Sleep(3000);
            ISecurityKeysRepository securityKeysRepository =
                (ISecurityKeysRepository)ContextRegistry.GetContext()["SecurityKeysPairRepository"];
            //add authentication handler
            config.MessageHandlers.Add(new AuthenticationHandler(new UserAuthenticationService(securityKeysRepository)));
            
            log.Info("Application Initialized.");
        }
    }
}
