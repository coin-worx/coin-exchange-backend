
namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Specifies which actions are susbcribed by the user for authorization using MFA
    /// </summary>
    public class MfaSubscriptions
    {
        private int _userId;
        private bool _loginMfaEnabled = false;
        private bool _depositMfaEnabled = false;
        private bool _withdrawalMfaEnabled = false;
        private bool _placeOrderMfaEnabled = false;
        private bool _cancelOrderMfaEnabled = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscriptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscriptions(int userId)
        {
            _userId = userId;
        }

        /// <summary>
        /// Subscribe to Login MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeLoginMfa()
        {
            _loginMfaEnabled = true;
            return true;
        }

        /// <summary>
        /// Subscribe to Deposit MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeDepositMfa()
        {
            _depositMfaEnabled = true;
            return true;
        }

        /// <summary>
        /// Subscribe to Withdrawal MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeWithdrawalMfa()
        {
            _withdrawalMfaEnabled = true;
            return true;
        }

        /// <summary>
        /// Subscribe to Place Order MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribePlaceOrderMfa()
        {
            _placeOrderMfaEnabled = true;
            return true;
        }

        /// <summary>
        /// Subscribe to Cancel Order MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeCancelOrderMfa()
        {
            _cancelOrderMfaEnabled = true;
            return true;
        }

        /// <summary>
        /// User ID
        /// </summary>
        public int UserId
        {
            get { return _userId; }
            private set { _userId = value; }
        }

        /// <summary>
        /// Is MFA enabled for Login
        /// </summary>
        public bool LoginMfaEnabled
        {
            get { return _loginMfaEnabled; }
            private set { _loginMfaEnabled = value; }
        }

        /// <summary>
        /// Is MFA enabled for Deposit
        /// </summary>
        public bool DepositMfaEnabled
        {
            get { return _depositMfaEnabled; }
            private set { _depositMfaEnabled = value; }
        }

        /// <summary>
        /// Is MFA enabled for Withdrawal
        /// </summary>
        public bool WithdrawalMfaEnabled
        {
            get { return _withdrawalMfaEnabled; }
            private set { _withdrawalMfaEnabled = value; }
        }

        /// <summary>
        /// Is MFA enabled for Placing a new order
        /// </summary>
        public bool PlaceOrderMfaEnabled
        {
            get { return _placeOrderMfaEnabled; }
            set { _placeOrderMfaEnabled = value; }
        }

        /// <summary>
        /// Is MFA enabled for Cancelling an order
        /// </summary>
        public bool CancelOrderMfaEnabled
        {
            get { return _cancelOrderMfaEnabled; }
            set { _cancelOrderMfaEnabled = value; }
        }
    }
}
