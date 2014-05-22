using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Generates a new and unique Activation key
    /// </summary>
    public interface IActivationKeyGenerationService
    {
        string GenerateNewActivationKey();
    }
}
