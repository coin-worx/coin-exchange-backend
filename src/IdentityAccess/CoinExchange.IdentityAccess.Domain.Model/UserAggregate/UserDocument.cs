using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// user Document
    /// </summary>
    public class UserDocument
    {
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="documentId"></param>
        /// <param name="documentType"></param>
        /// <param name="documentPath"></param>
        public UserDocument(string userName, string documentId, string documentType, string documentPath)
        {
            UserName = userName;
            DocumentId = documentId;
            DocumentType = documentType;
            DocumentPath = documentPath;
        }

        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Document ID
        /// </summary>
        public string DocumentId { get; private set; }

        /// <summary>
        /// Document Type
        /// </summary>
        public string DocumentType { get; private set; }

        /// <summary>
        /// Document Path
        /// </summary>
        public string DocumentPath { get; private set; }
    }
}
