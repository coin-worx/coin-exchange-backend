using System;

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// VO to response in result of order request
    /// </summary>
    [Serializable]
    public class CancelOrderResponse
    {
       
        public CancelOrderResponse(bool pending, int count)
        {
            Pending = pending;
            Count = count;
        }

        /// <summary>
        /// Constructor with Reponse message
        /// </summary>
        /// <param name="count"></param>
        /// <param name="pending"></param>
        /// <param name="responseMessage"></param>
        public CancelOrderResponse(int count, bool pending, string responseMessage)
        {
            Count = count;
            Pending = pending;
            ResponseMessage = responseMessage;
        }

        //No. of order canceled
        public int Count { get; private set; }
        
        //if set, order(s) is/are pending cancellation
        public bool Pending { get; private set; }

        public string ResponseMessage { get; set; }
    }
}
