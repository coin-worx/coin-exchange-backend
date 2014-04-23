namespace CoinExchange.Trades.Application.OrderServices
{
    //Stub implementation of cancel order command validation interface
    public class StubbedCancelOrderCommandValidation:ICancelOrderCommandValidation
    {
        public bool ValidateCancelOrderCommand(Commands.CancelOrderCommand orderCommand)
        {
            return true;
        }
    }
}
