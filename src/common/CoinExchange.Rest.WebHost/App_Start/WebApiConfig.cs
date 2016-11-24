/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using CoinExchange.IdentityAccess.Application;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
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
            //var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/json");
            //config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.DependencyResolver = new SpringDependencyResolver(ContextRegistry.GetContext());
            ISecurityKeysRepository securityKeysRepository =
                (ISecurityKeysRepository)ContextRegistry.GetContext()["SecurityKeysPairRepository"];
            IUserRepository userRepository = (IUserRepository)ContextRegistry.GetContext()["UserRepository"];
            IIdentityAccessPersistenceRepository _persistenceRepository =
                (IIdentityAccessPersistenceRepository)
                    ContextRegistry.GetContext()["IdentityAccessPersistenceRepository"];
            //Add authentication handler
            config.MessageHandlers.Add(new AuthenticationHandler(new UserAuthenticationService(userRepository,
                securityKeysRepository, _persistenceRepository)));
            
            log.Info("Application Initialized.");
        }
    }
}
