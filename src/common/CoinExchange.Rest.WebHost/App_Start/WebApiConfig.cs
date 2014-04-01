using System.Linq;
using System.Web.Http;

namespace CoinExchange.Rest.WebHost.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "trades",
                routeTemplate: "trades",
                defaults: new { ApiController = "TradeResource", id = RouteParameter.Optional }
            );

           /* config.Routes.MapHttpRoute(
                name: "funds",
                routeTemplate: "funds",
                defaults: new { ApiController = "FundsResource", id = RouteParameter.Optional }
            );*/

            //return Json format
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}
