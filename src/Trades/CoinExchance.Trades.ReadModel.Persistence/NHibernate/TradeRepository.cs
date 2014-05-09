using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NHibernate.Linq;
using NHibernate.Type;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class TradeRepository : NHibernateSessionFactory,ITradeRepository
    {
        [Transaction(ReadOnly = true)]
        public IList<object> GetRecentTrades(string lastId, string pair)
        {
            IList<dynamic> objects = CurrentSession.QueryOver<TradeReadModel>().Select(t => t.ExecutionDateTime, t => t.Price, t => t.Volume).List<dynamic>();

            return objects.OrderBy(x => x[0]).ToList();
        }

        //[Transaction(ReadOnly = true)]
        //public IList<TradeReadModel> GetTraderTradeHistory(string traderId)
        //{
        //    return CurrentSession.Query<TradeReadModel>()
        //        .Where(trade => trade.BuyTraderId.Equals(traderId)||trade.SellTraderId.Equals(traderId))
        //        .AsQueryable()
        //        .ToList();
        //}

        [Transaction(ReadOnly = true)]
        public IList<object> GetTraderTradeHistory(string traderId)
        {
            return CurrentSession.QueryOver<TradeReadModel>().Select(t=>t.TradeId,t => t.ExecutionDateTime, t => t.Price, t => t.Volume,t=>t.CurrencyPair)
                .Where(trade => trade.BuyTraderId==traderId || trade.SellTraderId==traderId)
                .List<object>();
        }

        [Transaction(ReadOnly = true)]
        public IList<object> GetTraderTradeHistory(string traderId,DateTime start,DateTime end)
        {
            return CurrentSession.QueryOver<TradeReadModel>().Select(t => t.TradeId, t => t.ExecutionDateTime, t => t.Price, t => t.Volume, t => t.CurrencyPair)
                .Where(trade => trade.BuyTraderId == traderId || trade.SellTraderId == traderId)
                .List<object>();
        }
        [Transaction(ReadOnly = true)]
        public TradeReadModel GetById(string tradeId)
        {
            return CurrentSession.Get<TradeReadModel>(tradeId);
        }

        [Transaction(ReadOnly = true)]
        public IList<TradeReadModel> GetTradesBetweenDates(DateTime end, DateTime start,string currencyPair)
        {
            return
                CurrentSession.QueryOver<TradeReadModel>()
                    .Where(x => x.ExecutionDateTime <= end && x.ExecutionDateTime >= start && x.CurrencyPair==currencyPair).OrderBy(x => x.ExecutionDateTime).Desc
                    .List();
        }

        [Transaction(ReadOnly = true)]
        public object GetCustomDataBetweenDates(DateTime end, DateTime start,string currencyPair)
        {
            string sqlQuery = string.Format("SELECT COUNT(TradeId) as NumberOfTrades,max(Price) as high, min(Price)as low, SUM(Volume)as Volume, SUM(Volume*Price)/SUM(Volume) as vwap FROM coinexchange.trade WHERE ExecutionDateTime >='{0}' AND ExecutionDateTime <='{1}' AND CurrencyPair='{2}'", start.ToString("u"), end.ToString("u"),currencyPair);
            var result = CurrentSession.CreateSQLQuery(sqlQuery).List();
            return result[0];
        }
    }
}
