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
            System.Console.WriteLine(client.CreateOrder("xbtusd","market","buy",10));
            System.Console.ReadKey();
        }
    }
}
