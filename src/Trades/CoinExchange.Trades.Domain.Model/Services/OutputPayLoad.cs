using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Domain.Model.Services
{
    public enum OutPutPayLoadType
    {
        //Resharper disable InconsistentNaming
        BBO,
        ORDER_BOOK,
        DEPTH,
        ORDER,
        TRADE
        //Resharper enable InconsistentNaming
    }
    public class OutputPayLoad
    {
        public OutPutPayLoadType Type;
        public Depth Ask;
        public Depth Bid;
        public Order Order;
        public Trade Trade;
        public LimitOrderBook LimitOrderBook;
        public BBOListener BboListener;
        public string EventName;

        public OutputPayLoad()
        {
            
        }

        public OutputPayLoad CreatePayLoad(object payLoadObject,string eventName="")
        {
            OutputPayLoad payLoad=new OutputPayLoad();
            if (payLoadObject is Trade)
            {
                payLoad.Type=OutPutPayLoadType.TRADE;
                payLoad.Trade = payLoadObject as Trade;
            }
            else if (payLoadObject is Order)
            {
                payLoad.Type = OutPutPayLoadType.ORDER;
                payLoad.Order = payLoadObject as Order;
                payLoad.EventName = eventName;
            }
            else if (payLoadObject is LimitOrderBook)
            {
                payLoad.Type = OutPutPayLoadType.ORDER_BOOK;
                payLoad.LimitOrderBook = payLoadObject as LimitOrderBook;
            }
            else if (payLoadObject is BBOListener)
            {
                payLoad.Type = OutPutPayLoadType.BBO;
                payLoad.BboListener = payLoadObject as BBOListener;
            }
            return payLoad;
        }

        public OutputPayLoad CreatePayLoadForDepth(Depth ask, Depth bid)
        {
            OutputPayLoad payLoad = new OutputPayLoad();
            payLoad.Type=OutPutPayLoadType.DEPTH;
            payLoad.Ask = ask;
            payLoad.Bid = bid;
            return payLoad;
        }
    }
}
