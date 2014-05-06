using CoinExchange.Trades.Application.OrderServices.Commands;
namespace CoinExchange.Trades.Application.OrderServices
{
    //Stub implementation of cancel order command validation interface
    public class StubbedCancelOrderCommandValidation:ICancelOrderCommandValidation
    {
        public bool ValidateCancelOrderCommand(CancelOrderCommand orderCommand)
        {
            return true;
        }


        public string GetCurrencyPair(Domain.Model.OrderAggregate.OrderId orderId)
        {
            return "XBTUSD";
        }
    }
}
