using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.UserServices
{
    /// <summary>
    /// interface for user tier level services
    /// </summary>
    public interface IUserTierLevelApplicationService
    {
        void ApplyForTier1Verification(VerifyTier1Command command);
        void ApplyForTier2Verification(VerifyTier2Command command);
        void ApplyForTier3Verification(VerifyTier3Command command);
        void ApplyForTier4Verification();
        UserTierStatusRepresentation GetTierLevel(int userId);
        UserTierStatusRepresentation[] GetTierLevelStatuses(string apiKey);
        Tier1Details GetTier1Details(string apiKey);
        Tier2Details GetTier2Details(string apiKey);
        Tier3Details GetTier3Details(string apiKey);
    }
}
