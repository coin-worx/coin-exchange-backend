using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Specifications;

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// serves the purpose to add all the business logic rules for validating orders.
    /// </summary>
    public class OrderSpecification:ISpecification<Order>
    {
        private decimal _minVolume = 0;
        private decimal _maxVolume = 0;
        public bool IsSatisfiedBy(Order entity)
        {
            if(entity.Volume<=_minVolume||entity.Volume>=_minVolume)
                throw new Exception("Volume is too high or too low.");
            if (entity.OrderType == OrderType.Limit&&entity.Price==0)
                throw new Exception("Price is not specified with Limit Order");
            //TODO: have to place check if the trader has enough funds to place order.
            return true;
        }
    }
}
