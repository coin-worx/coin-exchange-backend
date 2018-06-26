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
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices.Commands;

namespace CoinExchange.Rest.WebHost
{
    /// <summary>
    /// Authenticates every Http Request
    /// </summary>
    public class AuthenticationHandler : DelegatingHandler
    {
        private IAuthenticationService _authenticationService;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        public AuthenticationHandler(IAuthenticationService service)
        {
            _authenticationService = service;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var headers = request.Headers;
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    if (_authenticationService.Authenticate(new AuthenticateCommand(auth[2], auth[1], auth[0], request.RequestUri.ToString(), auth[4], auth[3])))
                    {
                        // Looks like an authentic client! Create a principal.
                        var claims = new List<Claim>
                            {
                                            new Claim(ClaimTypes.SerialNumber, auth[4]),
                                            new Claim(ClaimTypes.AuthenticationMethod, AuthenticationMethods.Signature)
                            };

                        var principal = new ClaimsPrincipal(new[] { new ClaimsIdentity(claims, "Auth") });

                        Thread.CurrentPrincipal = principal;

                        if (HttpContext.Current != null)
                            HttpContext.Current.User = principal;
                        if (Log.IsDebugEnabled)
                        {
                            Log.Debug("Authenticated for call: URL="+request.RequestUri);
                        }

                    }
                }

                var response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    response.Headers.Add("Nounce", _authenticationService.GenerateNonce());
                }

                return response;
            }
            catch (Exception exception)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error("Authentication Exception",exception);
                }
                var response = request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Headers.Add("Nounce", _authenticationService.GenerateNonce());
                return response;
            }
        }
    }
}