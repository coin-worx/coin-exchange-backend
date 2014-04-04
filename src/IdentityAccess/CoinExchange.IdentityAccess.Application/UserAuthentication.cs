using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.Authentication;
using CoinExchange.IdentityAccess.Application.Authentication.Commands;

namespace CoinExchange.IdentityAccess.Application
{
    public class UserAuthentication:IAuthenticationService
    {
        public bool Authenticate(AuthenticateCommand command)
        {
            if (Nonce.IsValid(command.Nonce, command.Counter))
            {
                string computedHash = CalculateHash(command.Apikey, command.Uri, "AuroraBitCoinExchange");
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
