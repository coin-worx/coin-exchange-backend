using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Permission master data repository
    /// </summary>
    public interface IPermissionRepository
    {
        IList<Permission> GetAllPermissions();
    }
}
