namespace CoinExchange.Trades.Domain.Model.Order
{
    // ToDo: May need remove this class
    /// <summary>
    /// VO to get the required parameters for cancelling order request
    /// </summary>
    public class CancelOrderRequest
    {
        public string TraderId { get; private set; }
        /*txid may be an order tx id or a user reference id.An order tx id may be preceded by * to 
         cancel all open orders related to positions opened by the given order tx id*/
        public string TxId { get; private set; }

        public CancelOrderRequest(string traderId, string txId)
        {
            TraderId = traderId;
            TxId = txId;
        }
    }
}
