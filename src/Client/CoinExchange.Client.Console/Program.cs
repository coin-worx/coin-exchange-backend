using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinExchange.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = "http://rockblanc.cloudapp.net/dev";
            baseUrl = "http://localhost:51780";
            ApiClient client=new ApiClient(baseUrl);
            System.Console.WriteLine("Requesting....");
            //call methods available in api
            //System.Console.WriteLine(client.CreateOrder("XBTUSD", "limit", "sell", 5, 10));
            //System.Console.WriteLine(client.QueryClosedOrdersParams(false,"","","","",""));
            Scenario1(client);
            System.Console.ReadKey();
        }

        /// <summary>
        /// Testing scenario 1
        /// </summary>
        private static void Scenario1(ApiClient client)
        {
            //string currecyPair = "XBTUSD";
            ////Create orders
            //System.Console.WriteLine(client.CreateOrder(currecyPair, "limit", "buy", 10, 250));
            //System.Console.WriteLine(client.CreateOrder(currecyPair, "limit", "sell", 5, 252));
            //System.Console.WriteLine(client.CreateOrder(currecyPair, "market", "buy", 3));
            //System.Console.WriteLine(client.CreateOrder(currecyPair, "limit", "buy", 2, 253));
            //System.Console.WriteLine(client.CreateOrder(currecyPair, "market", "sell", 5));
            //System.Console.WriteLine(client.CreateOrder(currecyPair, "limit", "buy", 2, 250));
            //Thread.Sleep(5000);
            ScenarioResults(client);
        }

        /// <summary>
        /// Testing Scenario 2
        /// </summary>
        /// <param name="client"></param>
        private static void Scenario2(ApiClient client)
        {
            string currecyPair = "XBTUSD";
            JObject joe = JObject.Parse(client.CreateOrder(currecyPair, "market", "buy", 10));
            string orderId1 = joe.Property("OrderId").Value.ToString();
            string orderId2 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "sell", 5, 252)).Property("OrderId").Value.ToString();
            string orderId3 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 8, 245)).Property("OrderId").Value.ToString();
            string orderId4 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 7, 250)).Property("OrderId").Value.ToString();
            string orderId5 = JObject.Parse(client.CreateOrder(currecyPair, "market", "buy", 3)).Property("OrderId").Value.ToString();
            Thread.Sleep(2000);
            System.Console.WriteLine(client.CancelOrder(orderId2));
            string orderId6 = JObject.Parse(client.CreateOrder(currecyPair, "market", "buy", 10)).Property("OrderId").Value.ToString();
            string orderId7 = JObject.Parse(client.CreateOrder(currecyPair, "market", "sell", 5)).Property("OrderId").Value.ToString();
            System.Console.WriteLine(client.CancelOrder(orderId2));
            string orderId9 = client.CreateOrder(currecyPair, "market", "sell", 4);
            string orderId10 = client.CreateOrder(currecyPair, "market", "sell", 5);
            Thread.Sleep(5000);
            ScenarioResults(client);
        }

        /// <summary>
        /// Testing scenario 3
        /// </summary>
        /// <param name="client"></param>
        private static void Scenario3(ApiClient client)
        {
            string currecyPair = "XBTUSD";
            JObject joe = JObject.Parse(client.CreateOrder(currecyPair, "limit", "sell", 5, 252));
            string orderId1 = joe.Property("OrderId").Value.ToString();
            string orderId2 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 8, 245)).Property("OrderId").Value.ToString();
            string orderId3 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 7, 250)).Property("OrderId").Value.ToString();
            string orderId4 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 3, 250)).Property("OrderId").Value.ToString();
            string orderId5 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "sell", 5, 253)).Property("OrderId").Value.ToString();
            string orderId6 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 8, 240)).Property("OrderId").Value.ToString();
            string orderId7 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 7, 245)).Property("OrderId").Value.ToString();
            string orderId8 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 3, 247)).Property("OrderId").Value.ToString();
            Thread.Sleep(2000);
            System.Console.WriteLine(client.CancelOrder(orderId6));
            System.Console.WriteLine(client.CancelOrder(orderId1));
            string orderId9 = client.CreateOrder(currecyPair, "market", "sell", 9);
            System.Console.WriteLine(client.CancelOrder(orderId8));
            System.Console.WriteLine(client.CancelOrder(orderId7));
            System.Console.WriteLine(client.CancelOrder(orderId6));
            System.Console.WriteLine(client.CancelOrder(orderId5));
            System.Console.WriteLine(client.CancelOrder(orderId4));
            System.Console.WriteLine(client.CancelOrder(orderId3));
            System.Console.WriteLine(client.CancelOrder(orderId2));
            System.Console.WriteLine(client.CancelOrder(orderId1));
            Thread.Sleep(5000);
            ScenarioResults(client);
        }

        private static void ScenarioResults(ApiClient client)
        {
            System.Console.WriteLine("------RESULTS------");
            System.Console.WriteLine("------OPEN ORDERS------");
            System.Console.WriteLine(client.QueryOpenOrdersParams(true, ""));
            System.Console.WriteLine("------CLOSED ORDERS------");
            System.Console.WriteLine(client.QueryClosedOrdersParams(true, "", ""));
            System.Console.WriteLine("------TRADES------");
            System.Console.WriteLine(client.GetTradeHistory("", ""));
        }
    }
}
