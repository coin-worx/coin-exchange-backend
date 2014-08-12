using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockBboRetrievalService : IBboRetrievalService
    {
        public Tuple<decimal, decimal> GetBestBidBestAsk(string baseCurrency, string quoteCurrency)
        {
            throw new NotImplementedException();
        }
    }
}
