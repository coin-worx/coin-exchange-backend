using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.Services
{
    public interface IEventStore
    {
        bool StoreEvent(object blob);
        object GetEvent(string id);
    }
}
