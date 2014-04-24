using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.ReadModel.DTO;

namespace CoinExchange.Trades.ReadModel.Services
{
    public interface IOrderQueryService
    {
        List<OrderReadModel> GetOpenOrders(string traderId);
        List<OrderReadModel> GetClosedOrders(string traderId);
        List<OrderReadModel> GetAllOrderOfTrader(string traderId);
        OrderReadModel GetOrderById(string orderId);
    }
}
