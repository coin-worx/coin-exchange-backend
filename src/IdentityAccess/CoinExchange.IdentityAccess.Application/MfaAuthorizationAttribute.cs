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


﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using CoinExchange.IdentityAccess.Domain.Model.Services;
using Newtonsoft.Json.Linq;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application
{
    /// <summary>
    /// Checks user's authorization subscription and acts accordingly
    /// </summary>
    public class MfaAuthorizationAttribute : ActionFilterAttribute
    {
        // Get Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
              (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IMfaAuthorizationService _mfaAuthorizationService;
        private string _currentAction = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.Filters.ActionFilterAttribute"/> class.
        /// </summary>
        public MfaAuthorizationAttribute(string currentAction)
        {
            _mfaAuthorizationService = (IMfaAuthorizationService)ContextRegistry.GetContext().GetObject("MfaAuthorizationService");
            _currentAction = currentAction;
        }

        /// <summary>
        /// Checks Mfa Authorization for a user before the action executes
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string inputStream = "";
            using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            {
                inputStream = reader.ReadToEnd();
            }

            // The MFA code is sent by the user when MFA is susbcribed on any specific action(s)

            // If the user has logged into the web app, then the in the first turn, this attribute will return false, prompting 
            // the user to enter their MFA code that will just be sent to them via SMS or email.

            // If the user is communicating through an API client while using an API key, then they will provide a password 
            // with every call they have subscribed for TFA. The password will be set once using the web app by the user
            string mfaCode =
                (string)Newtonsoft.Json.JsonConvert.DeserializeObject<IDictionary<string, object>>(inputStream)["MfaCode"];

            var headers = HttpContext.Current.Request.Headers;
            string authValues = headers.Get("Auth");
            string apiKey = authValues.Split(',')[0];

            Tuple<bool, string> authorizationResponse = _mfaAuthorizationService.AuthorizeAccess(apiKey, _currentAction,
                                                                                                 mfaCode);
            if (authorizationResponse.Item1)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug(string.Format("Mfa Code verification Successful"));
                }
                base.OnActionExecuting(actionContext);
            }
            else
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug(string.Format("Mfa Code verification failed"));
                }
                actionContext.Response = new HttpResponseMessage();
                JObject jObject = new JObject();
                jObject.Add("Successful", authorizationResponse.Item1);
                jObject.Add("Message", authorizationResponse.Item2);
                actionContext.Response.Content = new StringContent(jObject.ToString());
            }
        }
    }
}
