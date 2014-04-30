using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// BBORepresentationList
    /// </summary>
    public class BBORepresentationList : IEnumerable<BBORepresentation>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Each slot contains the BBO representation, containg best prices, best volumes and their order counts
        /// </summary>
        private List<BBORepresentation> _bboRepresentations = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BBORepresentationList()
        {
            _bboRepresentations = new List<BBORepresentation>();
        }

        /// <summary>
        /// Adds the best bid and best offer
        /// </summary>
        /// <returns></returns>
        public bool AddBBO(string currencyPair, DepthLevel bestBid, DepthLevel bestAsk)
        {
            var bboRepresentation = Contains(currencyPair);
            if (bboRepresentation != null)
            {
                _bboRepresentations.Remove(bboRepresentation);
            }
            if ((bestBid.Price != null && bestBid.AggregatedVolume != null) || (bestAsk.Price != null && bestAsk.AggregatedVolume != null))
            {
                _bboRepresentations.Add(new BBORepresentation(currencyPair, 
                                                        // If price or volume values are null, just assign 0 as the best price and/or volume
                                                        bestBid.Price != null ? bestBid.Price.Value : 0,
                                                        bestBid.AggregatedVolume != null ? bestBid.AggregatedVolume.Value : 0,
                                                        bestBid.OrderCount, 
                                                        bestAsk.Price != null ? bestAsk.Price.Value : 0,
                                                        bestAsk.AggregatedVolume != null ? bestAsk.AggregatedVolume.Value : 0, 
                                                        bestAsk.OrderCount));
                return true;
            }
            return false;
        }

        public BBORepresentation Contains(string currencyPair)
        {
            foreach (BBORepresentation bboRepresentation in _bboRepresentations)
            {
                if (bboRepresentation.CurrencyPair == currencyPair)
                {
                    return bboRepresentation;
                }
            }
            return null;
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// GetEnumerator(specific)
        /// </summary>
        /// <returns></returns>
        IEnumerator<BBORepresentation> IEnumerable<BBORepresentation>.GetEnumerator()
        {
            foreach (BBORepresentation bboRepresentation in _bboRepresentations)
            {
                if (bboRepresentation == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return bboRepresentation;
            }
        }

        /// <summary>
        /// GetEnumerator(generic)
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (BBORepresentation bboRepresentation in _bboRepresentations)
            {
                if (bboRepresentation == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return bboRepresentation;
            }
        }

        #endregion
    }
}
