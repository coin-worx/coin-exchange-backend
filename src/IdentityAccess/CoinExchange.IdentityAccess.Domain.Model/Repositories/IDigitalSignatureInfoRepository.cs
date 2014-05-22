using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Domain.Model.Repositories
{
    /// <summary>
    /// Digital signature info repository
    /// </summary>
    public interface IDigitalSignatureInfoRepository
    {
        DigitalSignatureInfo GetByKeyDescription(string keyDescription);
        DigitalSignatureInfo GetByApiKey(string apiKey);
    }
}
