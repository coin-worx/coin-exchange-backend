using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// withdraw fees
    /// </summary>
    public class WithdrawFees
    {
        public Currency Currency { get; private set; }
        public decimal MinAmount { get; private set; }
        public decimal Fee { get; private set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public WithdrawFees()
        {
            
        }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="minAmount"></param>
        /// <param name="fee"></param>
        public WithdrawFees(Currency currency, decimal minAmount, decimal fee)
        {
            Currency = currency;
            MinAmount = minAmount;
            Fee = fee;
        }
    }
}
