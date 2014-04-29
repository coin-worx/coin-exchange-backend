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
    /// Represents the Depth levels in a simplified format
    /// </summary>
    public class DepthLevelRepresentationList : IEnumerable<Tuple<decimal, decimal, int>>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Contains slots as tuples and each tuple represents:
        /// 1. Volume
        /// 2. Price
        /// 3. OrderCount
        /// </summary>
        private Tuple<decimal, decimal, int>[] _depthLevelList = null;
        private int _size = 0;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="size"></param>
        public DepthLevelRepresentationList(int size)
        {
            _depthLevelList = new Tuple<decimal, decimal, int>[size];
        }

        /// <summary>
        /// Adds the depth level to the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="depthLevel"></param>
        /// <returns></returns>
        public bool AddDepthLevel(int index, DepthLevel depthLevel)
        {
            if (depthLevel.AggregatedVolume != null && depthLevel.Price != null)
            {
                _depthLevelList[index] = new Tuple<decimal, decimal, int>(depthLevel.AggregatedVolume.Value,
                                                                          depthLevel.Price.Value, depthLevel.OrderCount);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Flattens i.e., makes the specified slot null
        /// </summary>
        /// <returns></returns>
        public bool FlattenDepthLevel(int index)
        {
            _depthLevelList[index] = null;
            return true;
        }

        /// <summary>
        /// Size
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        /// <summary>
        /// DepthLevels representation
        /// </summary>
        public Tuple<decimal, decimal, int>[] DepthLevels
        {
            get { return _depthLevelList; }
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// GetEnumerator(specific)
        /// </summary>
        /// <returns></returns>
        IEnumerator<Tuple<decimal, decimal, int>> IEnumerable<Tuple<decimal, decimal, int>>.GetEnumerator()
        {
            foreach (Tuple<decimal, decimal, int> orderStats in _depthLevelList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (orderStats == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return orderStats;
            }
        }

        /// <summary>
        /// GetEnumerator(generic)
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (Tuple<decimal, decimal, int> orderStats in _depthLevelList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (orderStats == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return orderStats;
            }
        }

        #endregion
    }
}
