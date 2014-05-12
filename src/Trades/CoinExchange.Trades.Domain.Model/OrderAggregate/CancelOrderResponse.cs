using System;

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// VO to response in result of order request
    /// </summary>
    //[Serializable]
    public class CancelOrderResponse
    {
       
        public CancelOrderResponse(bool pending)
        {
            Pending = pending;
        }

        /// <summary>
        /// Constructor with Reponse message
        /// </summary>
        /// <param name="pending"></param>
        /// <param name="responseMessage"></param>
        public CancelOrderResponse(bool pending, string responseMessage)
        {
            Pending = pending;
            ResponseMessage = responseMessage;
        }
        //if set, order(s) is/are pending cancellation
        public bool Pending { get; private set; }

        public string ResponseMessage { get; set; }
    }
}
