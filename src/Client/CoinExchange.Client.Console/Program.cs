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
            System.Console.WriteLine(client.CreateOrder("xbtusd","limit","buy",10, 491));
            System.Console.ReadKey();
        }
    }
}
