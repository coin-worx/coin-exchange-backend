using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.ReadModel.DTO;

namespace CoinExchange.Trades.Application.TradeServices.Representation
{
    /// <summary>
    /// Trade Details representation
    /// </summary>
    public class TradeDetailsRepresentation
    {
        public OrderReadModel Order { get; private set; }
        public DateTime ExecutionDateTime { get; private set; }
        public decimal ExecutionPrice { get; private set; }
        public decimal Volume { get; private set; }
        public string TradeId { get; private set; }

        /// <summary>
        /// paramterized constructor
        /// </summary>
        /// <param name="order"></param>
        /// <param name="executionDateTime"></param>
        /// <param name="executionPrice"></param>
        public TradeDetailsRepresentation(OrderReadModel order, DateTime executionDateTime, decimal executionPrice,decimal volume,string tradeId)
        {
            Order = order;
            ExecutionDateTime = executionDateTime;
            ExecutionPrice = executionPrice;
            Volume = volume;
            TradeId = tradeId;
        }
    }
}
