using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.Application.OrderServices
{
    /// <summary>
    /// Real implementation of validation of cancel order command
    /// </summary>
    public class CancelOrderCommandValidation:ICancelOrderCommandValidation
    {
        private readonly IOrderRepository _orderRepository;
        public bool ValidateCancelOrderCommand(CancelOrderCommand orderCommand)
        {
            var order = _orderRepository.GetOrderById(orderCommand.OrderId.Id.ToString());
            if(order!=null)
            {
                if(order.TraderId.Equals(orderCommand.TraderId.Id.ToString(),StringComparison.InvariantCultureIgnoreCase))
                {
                    //check if order is already cancelled
                    if(order.Status.Equals(OrderState.Cancelled.ToString()))
                    {
                        throw new InvalidOperationException("Order is already cancelled");
                    }
                    //check if order is already filled
                    if(order.Status.Equals(OrderState.Complete.ToString()))
                    {
                        throw new InvalidOperationException("Order is filled, cannot be cancelled");
                    }
                    //this means command is valid
                    return true;
                }
                throw new InvalidOperationException("Order id doest not belong to the trader id.");
            }
            throw new ArgumentException("Invalid order id");
        }

        public CancelOrderCommandValidation(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }


        public string GetCurrencyPair(OrderId orderId)
        {
            return _orderRepository.GetOrderById(orderId.Id.ToString()).CurrencyPair;
        }
    }
}
