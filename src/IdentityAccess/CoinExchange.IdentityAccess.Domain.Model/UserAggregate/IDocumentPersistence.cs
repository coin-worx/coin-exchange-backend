using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Interface for handling doucment persistence
    /// </summary>
    public interface IDocumentPersistence
    {
        UserDocument PersistDocument(string filename, string path, MemoryStream stream,string documentType,int userId);
    }
}
