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
        public string key = "55555";
        public string secretkey = "s3cr3t";
        private string nonce = "";
        private string cnounce = "asdfgh";
        private int counter = 0;
        protected string _baseUrl = "";

        public ApiClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string HttpGetRequest(string url = "http://localhost:51780/api")
        {
            string message = "";
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                    request.Method = "GET";
                    request.ContentType = "application/json; charset=utf-8";
                    string hash = String.Format("{0}:{1}:{2}", key, url, secretkey).ToMD5Hash();
                    string split = String.Format("{0},{1},{2},{3},{4}", key, nonce, cnounce, ++counter, hash);
                    request.Headers.Add("Auth", split);
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
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public string GetTradeHistory(string start, string end)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("Start", start);
            jsonObject.Add("End", end);
            string url = _baseUrl + "/trades/tradehistory";
            return HttpPostRequest(jsonObject, url);
        }

        /// <summary>
        /// GetBalances
        /// </summary>
        /// <returns></returns>
        public string GetBalances()
        {
            string url = _baseUrl + "/funds/getbalances";
            return HttpGetRequest(url);
        }

        /// <summary>
        /// Query trades of specific txid
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public string QueryTrades(string orderId)
        {
            string url = _baseUrl + "/trades/querytrades";
            return HttpPostRequest(orderId, url);
        }

        /// <summary>
        /// Return trade volume of pair
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        public string GetTradeVolume(string pair)
        {
            string url = _baseUrl + "/trades/tradevolume";
            JObject obj = new JObject();
            obj.Add("", pair);
            return HttpPostRequest(obj,url);
        }

        /// <summary>
        /// Cancel the order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public string CancelOrder(string orderId)
        {
            string url = _baseUrl + "/orders/cancelorder";
            return HttpPostRequest(orderId, url);
        }

        /// <summary>
        /// Create user order
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="type"></param>
        /// <param name="side"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public string CreateOrder(string pair, string type, string side,decimal volume, decimal price = 0)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("Pair", pair);
            jsonObject.Add("Type", type);
            jsonObject.Add("Side", side);
            jsonObject.Add("Price", price);
            jsonObject.Add("Volume", volume);
            string url = _baseUrl + "/orders/createorder";
            return HttpPostRequest(jsonObject, url);
        }

        /// <summary>
        /// Query user open orders
        /// </summary>
        /// <param name="includeTrades"></param>
        /// <param name="userRefId"></param>
        /// <returns></returns>
        public string QueryOpenOrdersParams(bool includeTrades, string userRefId)
        {
            string url = _baseUrl + "/orders/openorders";
            return HttpPostRequest(includeTrades.ToString(),url);
        }

        /// <summary>
        /// Query Closed Orders
        /// </summary>
        /// <param name="includeTrades"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public string QueryClosedOrdersParams(bool includeTrades,string startTime, string endTime)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("includeTrades", includeTrades);
            jsonObject.Add("startTime", startTime);
            jsonObject.Add("endTime", endTime);
            string url = _baseUrl + "/orders/closedorders";
            return HttpPostRequest(jsonObject, url);
        }
        
        /// <summary>
        /// Gets teh Order Info
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public string QueryOrderInfo(string orderId)
        {
            string url = _baseUrl + "/orders/queryorders";
            return HttpPostRequest(orderId, url);
        }

        /// <summary>
        /// Returns the Order Book
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        public string GetOrderBook(string currencyPair)
        {
            string url = _baseUrl + "/marketdata/orderbook?currencyPair=" + currencyPair;
            return HttpGetRequest(url);
        }

        /// <summary>
        /// Returns the Depth
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        public string GetDepth(string currencyPair)
        {
            string url = _baseUrl + "/marketdata/depth?currencyPair=" + currencyPair;
            return HttpGetRequest(url);
        }

        #endregion
        
        /// <summary>
        /// Private call requests
        /// </summary>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        protected string HttpPostRequest(object param, string url)
        {
            string message = "";
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.KeepAlive = false;
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
                    Stream receiveStream = exe.Response.GetResponseStream();

                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    message = readStream.ReadToEnd();
                    exe.Response.Close();
                    readStream.Close();
                }
            }
            return message;
        }
     }
}
