using System;
namespace CoinExchange.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = "http://rockblanc.cloudapp.net/dev";
            ApiClient client=new ApiClient(baseUrl);
            System.Console.WriteLine("Requesting....");
            //call methods available in api
            System.Console.WriteLine(client.GetTradeVolume("XBTUSD"));
            System.Console.ReadKey();
        }
    }
}
