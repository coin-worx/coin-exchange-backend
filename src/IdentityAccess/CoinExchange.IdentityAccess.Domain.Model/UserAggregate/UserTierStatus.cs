namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// User Tier Level Status
    /// </summary>
    public class UserTierStatus
    {
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="username"></param>
        /// <param name="tier"></param>
        /// <param name="status"></param>
        public UserTierStatus(string username, Tier tier, Status status)
        {
            Username = username;
            Tier = tier;
            Status = status;
        }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Tier
        /// </summary>
        public Tier Tier { get; private set; }

        /// <summary>
        /// Status
        /// </summary>
        public Status Status { get; private set; }
    }
}
