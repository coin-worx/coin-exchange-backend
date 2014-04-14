using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Domain.Model.Services
{
    public interface IOrderIdGenerator
    {
        OrderId GenerateOrderId();
    }
}
