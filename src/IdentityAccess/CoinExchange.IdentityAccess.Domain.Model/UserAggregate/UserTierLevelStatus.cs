namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// User Tier Level Status
    /// </summary>
    public class UserTierLevelStatus
    {
        private int _id { get; set; }

        //default constructor
        public UserTierLevelStatus()
        {
            
        }

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tier"></param>
        /// <param name="status"></param>
        public UserTierLevelStatus(int userId, Tier tier, Status status)
        {
            UserId = userId;
            Tier = tier;
            Status = status;
        }

        /// <summary>
        /// Username
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// Tier
        /// </summary>
        public Tier Tier { get; private set; }

        /// <summary>
        /// Status
        /// </summary>
        public Status Status { get; set; }
    }
}
