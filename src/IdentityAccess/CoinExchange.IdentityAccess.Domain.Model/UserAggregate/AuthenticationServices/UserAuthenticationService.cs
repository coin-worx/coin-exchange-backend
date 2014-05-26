using System;
using CoinExchange.Common.Domain.Model;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices.Commands;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices
{
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
                if (String.CompareOrdinal(computedHash, command.Response) == 0) return true;
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
                User user = null;
                // ToDo: Get the User by the API Key after Bilal provides the method in the UserRepository
                //_userRepository.GetUserByUserName()

                if (securityKeysPair.SystemGenerated)
                {
                    if (user != null)
                    {
                        int activeWindow = securityKeysPair.StartDate.Millisecond + user.AutoLogout.Milliseconds;

                        if (securityKeysPair.StartDate.AddMilliseconds(activeWindow) < DateTime.Now)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (securityKeysPair.ExpirationDate < DateTime.Now)
                    {
                        foreach (var securityPermission in securityKeysPair.PermissionList)
                        {
                            // ToDo: Implement after Master Data is loaded by Bilal for Permissions. Need to verify 
                            // the request from the URI and then check for the corresponding permission
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
