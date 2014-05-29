using System;
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
                    }
                }

                var response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    response.Headers.Add("Nounce", _authenticationService.GenerateNonce());
                }

                return response;
            }
            catch (Exception)
            {
                var response = request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Headers.Add("Nounce", _authenticationService.GenerateNonce());
                return response;
            }
        }
    }
}