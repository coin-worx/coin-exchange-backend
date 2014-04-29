using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// ChangeID
    /// </summary>
    [Serializable]
    public class ChangeId
    {
        private int _id;

        public ChangeId(int id)
        {
            _id = id;
        }

        public int Id
        {
            get { return _id; }
        }
    }
}
