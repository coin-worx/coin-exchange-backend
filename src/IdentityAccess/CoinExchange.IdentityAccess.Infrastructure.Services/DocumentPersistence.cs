using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Infrastructure.Services
{
    /// <summary>
    /// Implementation of document persistence
    /// </summary>
    public class DocumentPersistence:IDocumentPersistence
    {
        /// <summary>
        /// Persist document
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <param name="documentType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserDocument PersistDocument(string filename, string path, MemoryStream stream,string documentType,int userId)
        {
            string fullPath = string.Format(path + "\\{0}-{1}", userId, filename);
            FileStream fileStream=new FileStream(fullPath,FileMode.OpenOrCreate);
            stream.CopyTo(fileStream);
            return new UserDocument(userId,documentType,fullPath);
        }
    }
}
