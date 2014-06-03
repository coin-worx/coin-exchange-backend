using System;

namespace CoinExchange.IdentityAccess.Infrastructure.Persistence.Projection
{
    /// <summary>
    /// Projection for showing values to UI
    /// </summary>
    public class SecurityKeyPairList
    {
        public string KeyDescription { get;  private set; }
        public DateTime? ExpirationDate { get; private set; }
        public DateTime LastModified { get;  private set; }
        public DateTime CreationDateTime { get;  private set; }

        public SecurityKeyPairList()
        {
            
        }

        public SecurityKeyPairList(string keyDescription, DateTime expirationDate, DateTime lastModified, DateTime creationDateTime)
        {
            KeyDescription = keyDescription;
            ExpirationDate = expirationDate;
            LastModified = lastModified;
            CreationDateTime = creationDateTime;
        }
    }
}
