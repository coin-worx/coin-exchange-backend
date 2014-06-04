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
        public Tier3Param()
        {
            
        }

        public string Ssn { get; set; }
        public string Nin { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="ssn"></param>
        /// <param name="nin"></param>
        /// <param name="documentType"></param>
        public Tier3Param(string ssn, string nin, string documentType, string filename)
        {
            Ssn = ssn;
            Nin = nin;
            DocumentType = documentType;
            FileName = filename;
        }

        /// <summary>
        /// Custom to string method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("SSN:{0}, Nin:{1}, DocumentType:{2}, FileName:{3}", Ssn, Nin,
                DocumentType, FileName);
        }
    }
}
