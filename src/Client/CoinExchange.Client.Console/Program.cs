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
            string currecyPair = "XBTUSD";
            //Create orders
            System.Console.WriteLine(client.CreateOrder(currecyPair, "limit", "buy", 10, 250));
            System.Console.WriteLine(client.CreateOrder(currecyPair, "limit", "sell", 5, 252));
            System.Console.WriteLine(client.CreateOrder(currecyPair, "market", "buy", 3));
            System.Console.WriteLine(client.CreateOrder(currecyPair, "limit", "buy", 2, 253));
            System.Console.WriteLine(client.CreateOrder(currecyPair, "market", "sell", 5));
            System.Console.WriteLine(client.CreateOrder(currecyPair, "limit", "buy", 2, 250));

            //Get trades
            System.Console.WriteLine("-------Trades---------");
            System.Console.WriteLine(client.GetTradeHistory("","",true,"",""));
        }

        /// <summary>
        /// Testing Scenario 2
        /// </summary>
        /// <param name="client"></param>
        private void Scenario2(ApiClient client)
        {
            
        }

        /// <summary>
        /// Testing scenario 3
        /// </summary>
        /// <param name="client"></param>
        private static void Scenario3(ApiClient client)
        {
            string currecyPair = "XBTUSD";
            string orderId1 = client.CreateOrder(currecyPair, "limit", "sell", 5, 252);
            string orderId2 = client.CreateOrder(currecyPair, "limit", "buy", 8, 245);
            string orderId3 = client.CreateOrder(currecyPair, "limit", "buy", 7, 250);
            string orderId4 = client.CreateOrder(currecyPair, "limit", "buy", 3, 250);
            string orderId5 = client.CreateOrder(currecyPair, "limit", "sell", 5, 253);
            string orderId6 = client.CreateOrder(currecyPair, "limit", "buy", 8, 240);
            string orderId7 = client.CreateOrder(currecyPair, "limit", "buy", 7, 245);
            string orderId8 = client.CreateOrder(currecyPair, "limit", "buy", 3, 247);
            client.CancelOrder(orderId6);
            client.CancelOrder(orderId1);
            string orderId9 = client.CreateOrder(currecyPair, "market", "sell", 9);
            client.CancelOrder(orderId8);
            client.CancelOrder(orderId7);
            client.CancelOrder(orderId6);
            client.CancelOrder(orderId5);
            client.CancelOrder(orderId4);
            client.CancelOrder(orderId3);
            client.CancelOrder(orderId2);
            client.CancelOrder(orderId1);
            
        }
    }
}
