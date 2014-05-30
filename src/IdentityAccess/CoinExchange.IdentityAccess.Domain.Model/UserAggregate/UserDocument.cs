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
        //id for database
        private int _id { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public UserDocument()
        {
            
        }

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentType"></param>
        /// <param name="documentPath"></param>
        public UserDocument(int userId, string documentType, string documentPath)
        {
            UserId = userId;
            DocumentType = documentType;
            DocumentPath = documentPath;
        }

        /// <summary>
        /// Username
        /// </summary>
        public int UserId { get; set; }

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
