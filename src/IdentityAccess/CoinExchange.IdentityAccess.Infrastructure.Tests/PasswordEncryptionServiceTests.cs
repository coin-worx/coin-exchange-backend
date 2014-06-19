using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Infrastructure.Services;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Infrastructure.Tests
{
    [TestFixture]
    class PasswordEncryptionServiceTests
    {
        [Test]
        public void EncryptPasswordTest_EncryptsTwoPasswordAndCheckifNotNullAndLength_VerifiesThroughHashedValues()
        {
            PasswordEncryptionService passwordEncryption = new PasswordEncryptionService();
            string password = "23bv56hh67uj45gt32dx12az";
            string encryptPassword = passwordEncryption.EncryptPassword(password);
            Assert.IsNotNull(encryptPassword);
            Assert.AreNotEqual(password, encryptPassword);

            string secondPassword = "43nb87sd54fg45gt98uj11sz45hg12sz";
            string secondEncryptPassword = passwordEncryption.EncryptPassword(secondPassword);
            Assert.IsNotNull(secondEncryptPassword);
            Assert.AreNotEqual(secondPassword, secondEncryptPassword);

            Assert.AreEqual(encryptPassword.Length, secondEncryptPassword.Length);
        }

        [Test]
        public void EncryptPasswordTwiceAndCompareTest_EncryptsAPasswordTwiceAndThenComparesBothToBeTheSame_FailsIfNotEqual()
        {
            PasswordEncryptionService passwordEncryption = new PasswordEncryptionService();
            string password = "23bv56hh67uj";
            string hasedPassword = passwordEncryption.EncryptPassword(password);
            Assert.IsNotNull(hasedPassword);

            string samePassword = "23bv56hh67uj";
            Assert.IsTrue(passwordEncryption.VerifyPassword(samePassword, hasedPassword));
        }

        [Test]
        public void CompareFalseEncrytionTest_EncryptsAPasswordTwiceAndThenComparesBothToBeDifferent_FailsIfTheyAreEqual()
        {
            PasswordEncryptionService passwordEncryption = new PasswordEncryptionService();
            string password = "23bv56hh67uj";
            string encryptPassword = passwordEncryption.EncryptPassword(password);
            Assert.IsNotNull(encryptPassword);

            // Last letter is different
            string secondPassword = "23bv56hh67ut";
            string secondEncryptPassword = passwordEncryption.EncryptPassword(secondPassword);
            Assert.IsNotNull(secondEncryptPassword);

            Assert.AreNotEqual(encryptPassword, secondEncryptPassword);
        }

        [Test]
        public void CompareFalseEncrytionTest_EncryptsAPasswordTwiceAndThenComparesBothToBeDifferentByVerifyMethod_FailsIfTheyAreEqual()
        {
            PasswordEncryptionService passwordEncryption = new PasswordEncryptionService();
            string password = "23bv56hh67uj";
            string hashedPassword = passwordEncryption.EncryptPassword(password);
            Assert.IsNotNull(hashedPassword);

            // Last letter is different
            string secondPassword = "23bv56hh67ut";
            Assert.IsFalse(passwordEncryption.VerifyPassword(secondPassword, hashedPassword));
        }
    }
}
