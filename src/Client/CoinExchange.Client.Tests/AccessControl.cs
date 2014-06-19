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
        private string _userName;
        private string _password;

        public AccessControl(IdentityAccessClient client, string password, string userName)
        {
            _client = client;
            this._password = password;
            this._userName = userName;
        }

        public string CreateAndActivateUser(string username,string password,string email)
        {
            string activationcode = _client.SignUp(email, username, password, "pakistan",
                TimeZone.CurrentTimeZone, "");
            string code = JsonConvert.DeserializeObject<string>(activationcode);
            Console.WriteLine(_client.ActivateUser(username, password, code));
            return "Activated";
        }

        public string Login(string username,string password)
        {
            string login = _client.Login(username, password);
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
