using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CoinExchange.Client.Tests
{
    public class AccessControl
    {
        private IdentityAccessClient _client;
        private string userName;
        private string password;

        public AccessControl(IdentityAccessClient client, string password, string userName)
        {
            _client = client;
            this.password = password;
            this.userName = userName;
        }

        public string Login()
        {
            string activationcode = _client.SignUp("user@user.com", userName, password, "pakistan",
                TimeZone.CurrentTimeZone, "");
            string code = JsonConvert.DeserializeObject<string>(activationcode);
            Console.WriteLine(_client.ActivateUser(userName, password, code));
            string login = _client.Login(userName, password);
            Console.WriteLine(login);
            dynamic keys = JsonConvert.DeserializeObject<dynamic>(login);
            _client.key = keys.ApiKey;
            _client.secretkey = keys.SecretKey;
            return "";
        }

        public PermissionRepresentation[] ListPermissions()
        {
            return JsonConvert.DeserializeObject<PermissionRepresentation[]>(_client.ListPermissions());
        }

        public string GetSecurityPairs()
        {
            return _client.KeyPairList();
        }

        public string CreateSecurityKeyPair(string keydescription,PermissionRepresentation[] permissions)
        {
            return _client.CreateKey(keydescription,permissions);
        }
    }
}
