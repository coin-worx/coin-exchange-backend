
namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Specifies the Status of a subscription
    /// </summary>
    public class MfaSubscriptionStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscriptionStatus()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscriptionStatus(int userId, MfaSubscription mfaSubscription, bool enabled)
        {
            UserId = userId;
            MfaSubscription = mfaSubscription;
            Enabled = enabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscriptionStatus(string apiKey, MfaSubscription mfaSubscription, bool enabled)
        {
            ApiKey = apiKey;
            MfaSubscription = mfaSubscription;
            Enabled = enabled;
        }

        /// <summary>
        /// Primary key of database
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// API key in case the subscription is for an API Key
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// User ID
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// MFA Subscription
        /// </summary>
        public MfaSubscription MfaSubscription { get; private set; }

        /// <summary>
        /// Is subscription enabled
        /// </summary>
        public bool Enabled { get; set; }
    }
}
