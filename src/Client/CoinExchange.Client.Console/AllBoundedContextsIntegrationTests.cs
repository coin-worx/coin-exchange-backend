using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Client.Tests;

namespace CoinExchange.Client.Console
{
    /// <summary>
    /// Contains tests that test all the bounded contexts running mutually
    /// </summary>
    public class AllBoundedContextsIntegrationTests
    {
        private AccessControl _accessControl;
        private IdentityAccessClient _identityAccessClient;
        
        private string _baseUrlCloud = "http://rockblanc.cloudapp.net/test/v1";
        private string _baseUrlLocalhost = "http://localhost:51780/v1";
        private FundsClient _fundsClient;
        private string _username = "rodholt";
        private string _password = "mclaren";
        private string _email = "rodholt@mclaren.com";
        private string _baseCurrency = "BTC";
        private string _quoteCurrency = "USD";

        public AllBoundedContextsIntegrationTests()
        {
            _fundsClient = new FundsClient(_baseUrlLocalhost);
            _identityAccessClient = new IdentityAccessClient(_baseUrlLocalhost);
            
            _accessControl = new AccessControl(_identityAccessClient, _password, _username);
        }

        public void Initialization()
        {
            UserLogin();

            _fundsClient.key = _identityAccessClient.key;
            _fundsClient.secretkey = _identityAccessClient.secretkey;

            //MakeDeposit();
            SendOrders();
            ApplyForTier1();
            GetLimits();
        }

        private void UserLogin()
        {
            //_accessControl.CreateAndActivateUser(_username, _password, _email);
            _accessControl.Login(_username, _password);            
        }

        private void MakeDeposit()
        {
            _fundsClient.MakeDeposit(_baseCurrency, 10);
            _fundsClient.MakeDeposit(_quoteCurrency, 10000);
        }

        private void ApplyForTier1()
        {
            System.Console.WriteLine(_identityAccessClient.ApplyForTierLevel1("N/A", "N/A", "N/A"));
        }

        private void GetLimits()
        {
            System.Console.WriteLine(_fundsClient.GetDepositLimits(_baseCurrency));
            System.Console.WriteLine(_fundsClient.GetWithdrawalLimits(_baseCurrency));
        }

        private void SendOrders()
        {
            System.Console.WriteLine(_identityAccessClient.CreateOrder(_baseCurrency + _quoteCurrency, "limit", "buy", 2, 250));
        }
    }
}
