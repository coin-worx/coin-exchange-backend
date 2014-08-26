using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CoinExchange.Client.Tests
{
    public class FundsClient : ApiClient
    {
        private string _baseUrl;

        public FundsClient(string baseUrl) : base(baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string MakeDeposit(string currency, int amount)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("Currency", currency);
            jsonObject.Add("Amount", amount);
            string url = _baseUrl + "/funds/makedeposit";
            return HttpPostRequest(jsonObject, url);
        }

        public string GetDepositLimits(string currency)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("Currency", currency);
            string url = _baseUrl + "/funds/getdepositlimits";
            return HttpPostRequest(jsonObject, url);
        }

        public string GetWithdrawalLimits(string currency)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("Currency", currency);
            string url = _baseUrl + "/funds/getwithdrawlimits";
            return HttpPostRequest(jsonObject, url);
        }

        public string ApiKey { get; set; }

        public string SecretKey { get; set; }
    }
}
