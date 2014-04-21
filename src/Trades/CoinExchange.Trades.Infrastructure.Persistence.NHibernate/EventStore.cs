using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Infrastructure.Persistence
{
    public class EventStore
    {
        public string EventName;
        public object Aggrgate;
        public object Details;
    }
}
