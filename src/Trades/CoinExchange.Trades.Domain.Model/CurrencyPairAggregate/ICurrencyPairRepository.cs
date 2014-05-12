using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.CurrencyPairAggregate
{
    /// <summary>
    /// Currency pair repository interface
    /// </summary>
    public interface ICurrencyPairRepository
    {
        CurrencyPair GetById(string currencyPair);
        IList<CurrencyPair> GetTradeableCurrencyPairs();
    }
}
