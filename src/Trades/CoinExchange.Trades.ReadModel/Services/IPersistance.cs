using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.ReadModel.Services
{
    public interface IPersistance
    {
        void SaveOrUpdate(object readModelObject);
    }
}
