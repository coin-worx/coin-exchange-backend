using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinExchange.Client
{
    public class ApiClient
    {
        private string key = "123456";
        private string secretkey = "AuroraBitCoinExchange";
        private string nonce = "";
        private string cnounce = "asdfgh";
        private int counter = 0;
        private string _baseUrl = "";

        public ApiClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string TestPrivate(string url = "http://localhost:51780/api", params object[] a_params)
        {
            string message = "";
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json; charset=utf-8";
                    string hash = String.Format("{0}:{1}:{2}", key, url, secretkey).ToMD5Hash();
                    string split = String.Format("{0},{1},{2},{3},{4}", key, nonce, cnounce, ++counter, hash);
                    request.Headers.Add("Auth", split);
                    using (var writer = new StreamWriter(request.GetRequestStream()))
                    {
                        JObject joe = new JObject();
                        if (a_params != null)
                        {
                            if (a_params.Length > 0)
                            {
                                JArray props = new JArray();
                                foreach (var p in a_params)
                                {
                                    props.Add(p);
                                    joe.Add(new JProperty("", props));
                                    string json = JsonConvert.SerializeObject(p);
                                    writer.Write(json);
                                }
                            }
                        }
                    }
                    HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                    // Get the stream associated with the response.
                    Stream receiveStream = response.GetResponseStream();

                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    message = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                    return message;
                }
                catch (WebException exe)
                {
                    nonce = exe.Response.Headers["Nounce"];
                    message = exe.Response.ToString();
                }
             
            }
            return message;

        }
        #region Private Calls Methods
        /// <summary>
        /// Return Trade History
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="type"></param>
        /// <param name="trades"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public string GetTradeHistory(string offset, string type, bool trades, string start, string end)
        {
            JObject jsonObject=new JObject();
            jsonObject.Add("Offset", offset);
            jsonObject.Add("Type", type);
            jsonObject.Add("Trades", trades);
            jsonObject.Add("Start", start);
            jsonObject.Add("End", end);
            string url = _baseUrl + "/trades/tradehistory";
            return RequestServer(jsonObject,url);
        }
        /// <summary>
        /// Query trades of specific txid
        /// </summary>
        /// <param name="txId"></param>
        /// <param name="includeTrades"></param>
        /// <returns></returns>
        public string QueryTrades(string txId, bool includeTrades)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("txId", txId);
            jsonObject.Add("includeTrades", includeTrades);
            string url = _baseUrl + "/trades/querytrades";
            return RequestServer(jsonObject, url);
        }
        /// <summary>
        /// Return trade volume of pair
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        public string GetTradeVolume(string pair)
        {
            string url = _baseUrl + "/trades/tradevolume";
            return TestPrivate(url,pair);
        }
        /// <summary>
        /// Cancel the order
        /// </summary>
        /// <param name="txid"></param>
        /// <returns></returns>
        public string CancelOrder(string txid)
        {
            string url = _baseUrl + "/orders/cancelorder";
            return TestPrivate(url, txid);
        }
        /// <summary>
        /// Create user order
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="type"></param>
        /// <param name="side"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public string CreateOrder(string pair, string type, string side,decimal volume, decimal price=0)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("Pair", pair);
            jsonObject.Add("Type", type);
            jsonObject.Add("Side", side);
            jsonObject.Add("Price", price);
            jsonObject.Add("Volume", volume);
            string url = _baseUrl + "/orders/createorder";
            return RequestServer(jsonObject, url);
        }
        /// <summary>
        /// Query user open orders
        /// </summary>
        /// <param name="includeTrades"></param>
        /// <param name="userRefId"></param>
        /// <returns></returns>
        public string QueryOpenOrdersParams(bool includeTrades, string userRefId)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("includeTrades", includeTrades);
            jsonObject.Add("userRefId", userRefId);
            string url = _baseUrl + "/orders/openorders";
            return RequestServer(jsonObject, url);
        }
        /// <summary>
        /// Query user's closed orders
        /// </summary>
        /// <param name="includeTrades"></param>
        /// <param name="userRefId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="offset"></param>
        /// <param name="closeTime"></param>
        /// <returns></returns>
        public string QueryClosedOrdersParams(bool includeTrades, string userRefId, string startTime, string endTime,
            string offset,
            string closeTime)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("includeTrades", includeTrades);
            jsonObject.Add("userRefId", userRefId);
            jsonObject.Add("startTime", startTime);
            jsonObject.Add("endTime", endTime);
            jsonObject.Add("offset", offset);
            jsonObject.Add("closeTime", closeTime);
            string url = _baseUrl + "/orders/closedorders";
            return RequestServer(jsonObject, url);
        }
        #endregion
        /// <summary>
        /// Private call requests
        /// </summary>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string RequestServer(JObject param,string url)
        {
            string message = "";
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json; charset=utf-8";
                    string hash = String.Format("{0}:{1}:{2}", key, url, secretkey).ToMD5Hash();
                    string split = String.Format("{0},{1},{2},{3},{4}", key, nonce, cnounce, ++counter, hash);
                    request.Headers.Add("Auth", split);
                    using (var writer = new StreamWriter(request.GetRequestStream()))
                    {
                        string json = JsonConvert.SerializeObject(param);
                        writer.Write(json);
                    }
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    // Get the stream associated with the response.
                    Stream receiveStream = response.GetResponseStream();

                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    message = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                    return message;
                }
                catch (WebException exe)
                {
                    nonce = exe.Response.Headers["Nounce"];
                    message = exe.Response.ToString();
                }
            }
            return message;
        }
     }
}
