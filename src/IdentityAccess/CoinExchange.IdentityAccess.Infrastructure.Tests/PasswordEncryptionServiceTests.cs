/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
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
