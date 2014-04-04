using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.Authentication
{
    /// <summary>
    /// Serves the purpose of maintaing and genrating nonces
    /// </summary>
    class Nonce
    {
        private static ConcurrentDictionary<string, Tuple<int, DateTime>>
          nonces = new ConcurrentDictionary<string, Tuple<int, DateTime>>();

        public static string Generate()
        {
            byte[] bytes = new byte[16];

            using (var rngProvider = new RNGCryptoServiceProvider())
            {
                rngProvider.GetBytes(bytes);
            }

            string nonce = bytes.ToMD5Hash();

            nonces.TryAdd(nonce, new Tuple<int, DateTime>(0, DateTime.Now.AddMinutes(10)));

            return nonce;
        }

        public static bool IsValid(string nonce, string nonceCount)
        {
            Tuple<int, DateTime> cachedNonce = null;
            nonces.TryGetValue(nonce, out cachedNonce);

            if (cachedNonce != null) // nonce is found
            {
                // nonce count is greater than the one in record
                if (Int32.Parse(nonceCount) > cachedNonce.Item1)
                {
                    // nonce has not expired yet
                    if (cachedNonce.Item2 > DateTime.Now)
                    {
                        // update the dictionary to reflect the nonce count just received in this request
                        nonces[nonce] = new Tuple<int, DateTime>(Int32.Parse(nonceCount),cachedNonce.Item2);

                        // Every thing looks ok - server nonce is fresh and nonce count seems to be 
                        // incremented. Does not look like replay.
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
