using System;
using System.Collections.Generic;
using CoinExchange.Common.Domain.Model;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices.Commands;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices
{
    /// <summary>
    /// User Authentication Service
    /// </summary>
    public class UserAuthenticationService : IAuthenticationService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ISecurityKeysRepository _securityKeysRepository = null;
        private IUserRepository _userRepository = null;

        /// <summary>
        /// Initializes with the Secrutiy Keys Repository
        /// </summary>
        /// <param name="userRepository"> </param>
        /// <param name="securityKeysRepository"></param>
        public UserAuthenticationService(IUserRepository userRepository, ISecurityKeysRepository securityKeysRepository)
        {
            _securityKeysRepository = securityKeysRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Authenticates a request
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool Authenticate(AuthenticateCommand command)
        {
            if (Nonce.IsValid(command.Nonce, command.Counter))
            {
                string computedHash = CalculateHash(command.Apikey, command.Uri, Constants.GetSecretKey(command.Apikey));
                if (Log.IsDebugEnabled)
                {
                    Log.Debug("Computed Hash:"+computedHash);
                    Log.Debug("Received Hash:" + command.Response);
                }
                if (String.CompareOrdinal(computedHash, command.Response) == 0)
                {
                    return ApiKeyValidation(command);
                }
            }
            return false;
        }

        /// <summary>
        /// Validates the credentials related to the API Key
        /// </summary>
        /// <returns></returns>
        private bool ApiKeyValidation(AuthenticateCommand authenticateCommand)
        {
            SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(authenticateCommand.Apikey);

            if (securityKeysPair != null)
            {
                User user = _userRepository.GetUserByUserName(securityKeysPair.UserName);
                if (user != null)
                {
                    // If the keys are system generated, then we only need to check the session timeout for the user
                    if (securityKeysPair.SystemGenerated)
                    {
                        int activeWindow = securityKeysPair.StartDate.Millisecond + user.AutoLogout.Milliseconds;

                        if (securityKeysPair.StartDate.AddMilliseconds(activeWindow) < DateTime.Now)
                        {
                            return true;
                        }
                    }
                    // Else we need to check the expiration date of the keys, and whetehr the user has permissions for 
                    // commencing with the desired operation
                    else
                    {
                        
                        if (securityKeysPair.ExpirationDate < DateTime.Now)
                        {
                            if (authenticateCommand.Uri.Contains("/orders/"))
                            {
                                if (authenticateCommand.Uri.Contains("cancelorder"))
                                {
                                    return securityKeysPair.ValidatePermission(PermissionsConstant.Cancel_Order);
                                }
                                else if (authenticateCommand.Uri.Contains("openorders"))
                                {
                                    return securityKeysPair.ValidatePermission(PermissionsConstant.Query_Open_Orders);
                                }
                                else if (authenticateCommand.Uri.Contains("closedorders"))
                                {
                                    return securityKeysPair.ValidatePermission(PermissionsConstant.Query_Closed_Orders);
                                }
                            }
                            else if (authenticateCommand.Uri.Contains("/trades/"))
                            {
                                if (authenticateCommand.Uri.Contains("tradehistory"))
                                {
                                    // ToDo: Discuss with Bilal where are Trades permissions
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Calculates Hash
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="uri"></param>
        /// <param name="secretkey"></param>
        /// <returns></returns>
        private string CalculateHash(string apikey,string uri,string secretkey)
        {
            return String.Format("{0}:{1}:{2}", apikey, uri, secretkey).ToMD5Hash();
        }
        
        /// <summary>
        /// Generates Nounce
        /// </summary>
        /// <returns></returns>
        public string GenerateNonce()
        {
            return Nonce.Generate();
        }
    }
}
