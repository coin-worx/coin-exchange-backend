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
            System.Console.WriteLine(client.CreateOrder("XBTUSD", "limit", "sell", 5, 10));
            //System.Console.WriteLine(client.QueryClosedOrdersParams(false,"","","","",""));
            System.Console.ReadKey();
        }
    }
}
