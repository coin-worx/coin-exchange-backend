namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Tier, a particular level of a subscription
    /// </summary>
    public class Tier
    {
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="tierName"></param>
        /// <param name="tierLevel"></param>
        public Tier(string tierName, string tierLevel)
        {
            TierName = tierName;
            TierLevel = tierLevel;
        }

        /// <summary>
        /// Tier's Name
        /// </summary>
        public string TierName { get; private set; }

        /// <summary>
        /// Tier's Level
        /// </summary>
        public string TierLevel { get; private set; }
    }
}
