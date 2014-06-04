using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// VO to take parameters for security key
    /// </summary>
    public class CreateSecurityKeyParam
    {
        public string KeyDescritpion { get; private set; }
        public bool EnableStartDate { get; private set; }
        public bool EnableEndDate { get; private set; }
        public bool EnableExpirationDate { get; private set; }
        public string EndDateTime { get; private set; }
        public string StartDateTime { get; private set; }
        public string ExpirationDateTime { get; private set; }
        public SecurityKeyPermissionsRepresentation[] Permissions { get; private set; }

    }
}
