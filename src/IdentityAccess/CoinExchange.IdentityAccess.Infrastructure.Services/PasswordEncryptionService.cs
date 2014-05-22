using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using BCrypt.Net;

namespace CoinExchange.IdentityAccess.Infrastructure.Services
{
    /// <summary>
    /// Service for Password Encryption
    /// </summary>
    public class PasswordEncryptionService : IPasswordEncryptionService
    {
        private const string HashingValue = "]3s`!^^";

        /// <summary>
        /// Encrypts the given string password and returns the encrypted password as string
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string EncryptPassword(string password)
        {
            // Using a hard-coded value with every password, so that if the databse is copromised by hackers, they don't know
            // the hard coded value
            return BCrypt.Net.BCrypt.HashPassword(password + HashingValue, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        /// <summary>
        /// Verify if the entered password is the same as the one in the database
        /// </summary>
        /// <returns></returns>
        public bool VerifyPassword(string passwordEntered, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(passwordEntered + HashingValue, hash);
        }
    }
}
