using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// VO to gather tier 3 requirements
    /// </summary>
    public class VerifyTier3Command
    {
        public string SystemGeneratedApiKey { get; private set; }
        public string SocialSecurityNumber { get; private set; }
        public string Nin { get; private set; }
        public string DocumentType { get; private set; }
        public string FileName { get; private set; }
        public MemoryStream DocumentStream { get; private set; }
        
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="systemGeneratedApiKey"></param>
        /// <param name="socialSecurityNumber"></param>
        /// <param name="nin"></param>
        /// <param name="documentType"></param>
        public VerifyTier3Command(string systemGeneratedApiKey, string socialSecurityNumber, string nin, string documentType, string fileName,MemoryStream documentStream)
        {
            //AssertionConcern.AssertNullOrEmptyString(FileName,"FileName not specified");
            SystemGeneratedApiKey = systemGeneratedApiKey;
            SocialSecurityNumber = socialSecurityNumber;
            Nin = nin;
            DocumentType = documentType;
            FileName = fileName;
            DocumentStream = documentStream;
        }
    }
}