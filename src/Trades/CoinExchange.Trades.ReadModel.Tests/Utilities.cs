using System;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.ReadModel.Tests
{
    public class DepthCheck
    {
        private Depth _depth;
        private int _bidDepthLevelCounter = 0;
        private int _askDepthLevelCounter = 0;

        public DepthCheck(Depth depth)
        {
            _depth = depth;
        }

        public bool VerifyDepth(DepthLevel depthLevel, Price price, int count, Volume volume)
        {
            bool matched = true;
            if (depthLevel.Price != null)
            {
                if (!depthLevel.Price.Equals(price))
                {
                    Console.WriteLine("Price = " + depthLevel.Price.Value);
                    matched = false;
                }
                if (depthLevel.OrderCount != count)
                {
                    Console.WriteLine("OrderCount = " + depthLevel.OrderCount);
                    matched = false;
                }
                if (!depthLevel.AggregatedVolume.Equals(volume))
                {
                    Console.WriteLine("Volume = " + depthLevel.AggregatedVolume.Value);
                    matched = false;
                }
            }
            return matched;
        }

        public bool VerifyBid(Price price, int count, Volume volume)
        {
            bool result = VerifyDepth(_depth.BidLevels[_bidDepthLevelCounter], price, count, volume);
            _bidDepthLevelCounter++;
            return result;
        }

        public bool VerifyAsk(Price price, int count, Volume volume)
        {
            bool result = VerifyDepth(_depth.AskLevels[_askDepthLevelCounter], price, count, volume);
            _askDepthLevelCounter++;
            return result;
        }

        public void Reset()
        {
            _askDepthLevelCounter = 0;
            _bidDepthLevelCounter = 0;
        }
    }

    public class FillCheck
    {
        Volume _expectedFilledQty;
        Volume _expectedOpenQty;
        Price _expectedFilledCost;

        public bool VerifyFilled(Order order, Volume filledQuantity, Price filledPrice, Price filledCost)
        {
            bool asExpected = true;

            _expectedFilledQty = filledQuantity;
            _expectedOpenQty = order.Volume - _expectedFilledQty;
            _expectedFilledCost = new Price(filledPrice.Value * filledQuantity.Value);

            if (!order.FilledQuantity.Equals(_expectedFilledQty))
            {
                asExpected = false;
            }
            if (!order.OpenQuantity.Equals(_expectedOpenQty))
            {
                asExpected = false;
            }
            if (order.OpenQuantity.Value == 0 && order.OrderState != OrderState.Complete)
            {
                asExpected = false;
            }
            if (order.OpenQuantity.Value != 0 && order.OrderState != OrderState.PartiallyFilled)
            {
                asExpected = false;
            }
            return asExpected;
        }

        /// <summary>
        /// Checks if Order was partially or fully filled
        /// </summary>
        /// <param name="orderBook"></param>
        /// <param name="order"></param>
        /// <param name="isPartiallyFilled"> </param>
        /// <returns></returns>
        public bool FilledPartiallyOrComplete(LimitOrderBook orderBook, Order order, bool isPartiallyFilled)
        {
            bool matched = orderBook.AddOrder(order);

            if (isPartiallyFilled)
            {
                return order.OrderState == OrderState.PartiallyFilled;
            }
            else
            {
                return matched && order.OrderState == OrderState.Complete;
            }
        }
    }
}
