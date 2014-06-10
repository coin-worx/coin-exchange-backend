using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations
{
    /// <summary>
    /// VO to get permission info from user
    /// </summary>
    public class SecurityKeyPermissionsRepresentation
    {
        public Permission Permission { get;  set; }
        public bool Allowed { get;  set; }

        public SecurityKeyPermissionsRepresentation()
        {
            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="allowed"></param>
        /// <param name="permission"></param>
        public SecurityKeyPermissionsRepresentation(bool allowed, Permission permission)
        {
            Allowed = allowed;
            Permission = permission;
        }
    }
}
