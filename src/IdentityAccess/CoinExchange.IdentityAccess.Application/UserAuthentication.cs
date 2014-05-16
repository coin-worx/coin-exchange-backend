using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.IdentityAccess.Application.Authentication;
using CoinExchange.IdentityAccess.Application.Authentication.Commands;

namespace CoinExchange.IdentityAccess.Application
{
    public class UserAuthentication:IAuthenticationService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public bool Authenticate(AuthenticateCommand command)
        {
            if (Nonce.IsValid(command.Nonce, command.Counter))
            {
                string computedHash = CalculateHash(command.Apikey, command.Uri, Constants.GetSecretKey(command.Apikey));
                if (log.IsDebugEnabled)
                {
                    log.Debug("Computed Hash:"+computedHash);
                    log.Debug("Received Hash:" + command.Response);
                }
                if (String.CompareOrdinal(computedHash, command.Response) == 0) return true;
            }
            return false;
        }

        private string CalculateHash(string apikey,string uri,string secretkey)
        {
            return String.Format("{0}:{1}:{2}", apikey, uri, secretkey).ToMD5Hash();
        }
        
        public string GenerateNonce()
        {
            return Nonce.Generate();
        }
    }
}
