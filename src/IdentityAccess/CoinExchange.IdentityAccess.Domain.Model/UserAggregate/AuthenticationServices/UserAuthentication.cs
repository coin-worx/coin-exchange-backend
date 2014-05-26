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

        /// <summary>
        /// Initializes with the Secrutiy Keys Repository
        /// </summary>
        /// <param name="securityKeysRepository"></param>
        public UserAuthenticationService(ISecurityKeysRepository securityKeysRepository)
        {
            _securityKeysRepository = securityKeysRepository;
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
