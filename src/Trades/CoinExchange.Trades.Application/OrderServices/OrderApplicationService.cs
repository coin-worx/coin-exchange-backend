using System;
using System.IO;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Utility;
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
    public class OrderApplicationService : IOrderApplicationService
    {
        private ICancelOrderCommandValidation _commandValidationService;
        private IBalanceValidationService _balanceValidationService;

        public OrderApplicationService(ICancelOrderCommandValidation cancelOrderCommandValidation, 
            dynamic balanceValidationService)
        {
            _commandValidationService = cancelOrderCommandValidation;
            _balanceValidationService = balanceValidationService;
        }

        public CancelOrderResponse CancelOrder(CancelOrderCommand cancelOrderCommand)
        {
            try
            {
                // Verify cancel order command
                if (_commandValidationService.ValidateCancelOrderCommand(cancelOrderCommand))
                {
                    string currencyPair = _commandValidationService.GetCurrencyPair(cancelOrderCommand.OrderId);
                    OrderCancellation cancellation = new OrderCancellation(cancelOrderCommand.OrderId,
                                                                           cancelOrderCommand.TraderId,currencyPair);
                    InputDisruptorPublisher.Publish(InputPayload.CreatePayload(cancellation));
                    return new CancelOrderResponse(true, "Cancel Request Accepted");
                }
                return new CancelOrderResponse(false, new InvalidDataException("Invalid orderid").ToString());
            }
            catch (Exception exception)
            {
                return new CancelOrderResponse(false, exception.Message);
            }
        }

        public NewOrderRepresentation CreateOrder(CreateOrderCommand orderCommand)
        {
            IOrderIdGenerator orderIdGenerator = ContextRegistry.GetContext()["OrderIdGenerator"] as IOrderIdGenerator;

            Tuple<string, string> splittedCurrencyPairs = CurrencySplitterService.SplitCurrencyPair(orderCommand.Pair);
            
            Order order = OrderFactory.CreateOrder(orderCommand.TraderId, orderCommand.Pair,
                orderCommand.Type, orderCommand.Side, orderCommand.Volume, orderCommand.Price, orderIdGenerator);

            if (_balanceValidationService.FundsConfirmation(order.TraderId.Id, splittedCurrencyPairs.Item1,
                splittedCurrencyPairs.Item2, order.Volume.Value, order.Price.Value, order.OrderSide.ToString(),
                order.OrderId.Id))
            {
                InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order));
                return new NewOrderRepresentation(order);
            }
            throw new InvalidOperationException("Not Enough Balance for Trader with ID: " + orderCommand.TraderId);
        }
    }
}
