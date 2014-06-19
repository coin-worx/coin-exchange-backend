namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices.Commands
{
    public class AuthenticateCommand
    {
        public AuthenticateCommand(string cnonce, string nonce, string apikey, string uri, string response, string counter)
        {
            Cnonce = cnonce;
            Nonce = nonce;
            Apikey = apikey;
            Uri = uri;
            Response = response;
            Counter = counter;
        }

        public string Cnonce { get; private set; }
        public string Nonce { get; private set; }
        public string Apikey { get; private set; }
        public string Uri { get; private set; }
        public string Response { get; private set; }
        public string Counter { get; private set; }
        
    }
}
