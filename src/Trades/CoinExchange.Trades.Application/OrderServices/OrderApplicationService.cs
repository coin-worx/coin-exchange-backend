using System;
using System.IO;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Application.OrderServices
{
    /// <summary>
    /// Real implementation of order application service
    /// </summary>
    public class OrderApplicationService:IOrderApplicationService
    {
        private ICancelOrderCommandValidation _commandValidationService;
        
        public OrderApplicationService(ICancelOrderCommandValidation cancelOrderCommandValidation)
        {
            _commandValidationService = cancelOrderCommandValidation;
        }
        public CancelOrderResponse CancelOrder(CancelOrderCommand cancelOrderCommand)
        {
            // verify cancel order command
            if (_commandValidationService.ValidateCancelOrderCommand(cancelOrderCommand))
            {
                OrderCancellation cancellation = new OrderCancellation(cancelOrderCommand.OrderId,
                    cancelOrderCommand.TraderId);
                InputDisruptorPublisher.Publish(InputPayload.CreatePayload(cancellation));
                return new CancelOrderResponse(true,1);
            }
            throw new InvalidDataException("Invalid orderid");
        }

        public NewOrderRepresentation CreateOrder(CreateOrderCommand orderCommand)
        {
            IOrderIdGenerator orderIdGenerator = ContextRegistry.GetContext()["OrderIdGenerator"] as IOrderIdGenerator;
            Order order = OrderFactory.CreateOrder(orderCommand.TraderId, orderCommand.Pair,
                orderCommand.Type, orderCommand.Side, orderCommand.Volume, orderCommand.Price, orderIdGenerator);
            //TODO:Publish the order to disruptor
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order));
            return new NewOrderRepresentation(order);
        }
    }
}
