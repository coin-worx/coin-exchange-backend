using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Client
{
    public class AuthHeader
    {
        public AuthHeader(string nonce, string realm, string userName, string uri, string response, string method)
        {
            Nonce = nonce;
            Realm = realm;
            UserName = userName;
            Uri = uri;
            Response = response;
            Method = method;
           
        }

        public string Cnonce { get; private set; }
        public string Nonce { get; private set; }
        public string Realm { get; private set; }
        public string UserName { get; private set; }
        public string Uri { get; private set; }
        public string Response { get; private set; }
        public string Method { get; private set; }
        private int NounceCounter = 0;
    }
}
