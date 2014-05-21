using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// User Document List
    /// </summary>
    public class UserDocumentsList : IEnumerable<UserDocument>
    {
        private List<UserDocument> _userDocumentList = new List<UserDocument>();

        /// <summary>
        /// Add an element
        /// </summary>
        /// <returns></returns>
        internal bool AddTierStatus(UserDocument userDocument)
        {
            _userDocumentList.Add(userDocument);
            return true;
        }

        /// <summary>
        /// Remove an element
        /// </summary>
        /// <param name="userDocument"></param>
        /// <returns></returns>
        internal bool RemoveTierStatus(UserDocument userDocument)
        {
            _userDocumentList.Remove(userDocument);
            return true;
        }

        /// <summary>
        /// GetEnumerator - Specific
        /// </summary>
        /// <returns></returns>
        public IEnumerator<UserDocument> GetEnumerator()
        {
            foreach (var userDocument in _userDocumentList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (userDocument == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return userDocument;
            }
        }

        /// <summary>
        /// GetEnumerator - Generic
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
