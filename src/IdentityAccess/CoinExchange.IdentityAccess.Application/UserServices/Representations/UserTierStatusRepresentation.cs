using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.UserServices.Representations
{
    /// <summary>
    /// VO to represent tier status to user
    /// </summary>
    public class UserTierStatusRepresentation
    {
        public string Status { get; private set; }
        public Tier Tier { get; private set; }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="tier"></param>
        public UserTierStatusRepresentation(string status, Tier tier)
        {
            Status = status;
            Tier = tier;
        }
    }
}
