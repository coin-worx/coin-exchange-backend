using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.ReadModel.DTO;

namespace CoinExchange.Trades.ReadModel.Repositories
{
    public interface IOhlcRepository
    {
        OhlcReadModel GetOhlcByDateTime(DateTime dateTime);
    }
}
