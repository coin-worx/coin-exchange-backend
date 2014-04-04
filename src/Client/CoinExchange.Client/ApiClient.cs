using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;
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

        public string TestPrivate(string url = "http://localhost:51780/api", params object[] a_params)
        {
            string message = "";
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    //string url = "http://localhost:51780/api";
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
                       // object obj = "bilal";
                      //  string json = JsonConvert.SerializeObject(obj);
                       // writer.Write(json);
                        
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
        
    }
}
