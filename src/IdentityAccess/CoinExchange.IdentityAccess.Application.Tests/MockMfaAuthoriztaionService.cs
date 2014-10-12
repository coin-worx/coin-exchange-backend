using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;

namespace CoinExchange.IdentityAccess.Application.Tests
{
   public class MockMfaAuthorizationService : IMfaAuthorizationService
    {
       public Tuple<bool, string> AuthorizeAccess(string apiKey, string currentAction, string mfaCode)
       {
           return new Tuple<bool, string>(true, "");
       }
    }
}
