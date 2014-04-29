using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disruptor;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// In-memory Image for the Read-Side. All Memory images intended need to implmenet this interface
    /// </summary>
    public interface IMemoryImage : IEventHandler<byte[]>
    {
    }
}
