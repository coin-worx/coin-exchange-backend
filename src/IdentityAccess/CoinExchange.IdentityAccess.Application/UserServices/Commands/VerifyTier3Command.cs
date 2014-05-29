using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// VO to gather tier 3 requirements
    /// </summary>
    public class VerifyTier3Command
    {
        public string SystemGeneratedApiKey { get; private set; }
        public string SocialSecurityNumber { get; private set; }
        public string Cnic { get; private set; }
        public string DocumentType { get; private set; }
        public string DocumentExtension { get; private set; }
        public byte[] Attachement { get; private set; }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="systemGeneratedApiKey"></param>
        /// <param name="socialSecurityNumber"></param>
        /// <param name="cnic"></param>
        /// <param name="documentType"></param>
        /// <param name="documentExtension"></param>
        /// <param name="attachement"></param>
        public VerifyTier3Command(string systemGeneratedApiKey, string socialSecurityNumber, string cnic, string documentType, string documentExtension, byte[] attachement)
        {
            SystemGeneratedApiKey = systemGeneratedApiKey;
            SocialSecurityNumber = socialSecurityNumber;
            Cnic = cnic;
            DocumentType = documentType;
            DocumentExtension = documentExtension;
            Attachement = attachement;
        }
    }
}
