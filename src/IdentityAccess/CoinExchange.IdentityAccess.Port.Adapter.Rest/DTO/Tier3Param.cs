using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Paramters for collecting tier 3 parameters.
    /// </summary>
    public class Tier3Param
    {
        public string SocialSecurityNumber { get; private set; }
        public string Nin { get; private set; }
        public string DocumentType { get; private set; }
        public string FileName { get; private set; }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="socialSecurityNumber"></param>
        /// <param name="nin"></param>
        /// <param name="documentType"></param>
        /// <param name="documentExtension"></param>
        public Tier3Param(string socialSecurityNumber, string nin, string documentType, string filename)
        {
            SocialSecurityNumber = socialSecurityNumber;
            Nin = nin;
            DocumentType = documentType;
            FileName = filename;
        }
    }
}
