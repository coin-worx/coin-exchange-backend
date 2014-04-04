using System;
namespace CoinExchange.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiClient client=new ApiClient();
            System.Console.WriteLine("Requesting....");
            System.Console.WriteLine(client.TestPrivate("http://localhost:51780/test","BitcoinExchange"));
            System.Console.ReadKey();
        }
    }
}
