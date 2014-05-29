using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NHibernate;
using NHibernate.Mapping;
using NUnit.Framework;
using Spring.Data.NHibernate;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    [TestFixture]
    public class UserRepositoryPersistenceTests:AbstractConfiguration
    {
        private IIdentityAccessPersistenceRepository _persistenceRepository;
        private IUserRepository _userRepository;
        private ISessionFactory _sessionFactory;

        //properties will be injected based on type
        public IUserRepository UserRepository
        {
            set { _userRepository = value; }
        }
        public IIdentityAccessPersistenceRepository PersistenceRepository
        {
            set { _persistenceRepository = value; }
        }

        [Test]
        [Category("Integration")]
        public void CreateNewUser_PersistUserAndGetThatUserFromDB_BothUserAreSame()
        {
           User user=new User("NewUser","asdf","12345","xyz","user88@gmail.com",Language.English, TimeZone.CurrentTimeZone,new TimeSpan(1,1,1,1),DateTime.Now,"Pakistan","","2233344","1234");
            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
            user.IsUserBlocked=new IsUserBlocked(false);
           _persistenceRepository.SaveUpdate(user);
           User receivedUser = _userRepository.GetUserByUserName("NewUser");
            Assert.NotNull(receivedUser);
            Assert.AreEqual(user.Username,receivedUser.Username);
            Assert.AreEqual(user.Password, receivedUser.Password);
            Assert.AreEqual(user.PublicKey, receivedUser.PublicKey);
            Assert.AreEqual(user.Language, receivedUser.Language);
            Assert.AreEqual(user.AutoLogout, receivedUser.AutoLogout);
            Assert.AreEqual(user.TimeZone.ToString(),receivedUser.TimeZone.ToString());
            Assert.AreEqual(user.Country, receivedUser.Country);
            Assert.AreEqual(user.State, receivedUser.State);
            Assert.AreEqual(user.PhoneNumber, receivedUser.PhoneNumber);
            Assert.AreEqual(user.Address1,receivedUser.Address1);
            Assert.AreEqual(user.ActivationKey, receivedUser.ActivationKey);
            Assert.AreEqual(user.IsActivationKeyUsed.Value,true);
            Assert.AreEqual(user.IsUserBlocked.Value, false);
        }

        [Test]
        [Category("Integration")]
        public void CreateNewUser_PersistUserAndGetThatUserByEmailFromDatabase_BothUserAreSame()
        {
            User user = new User("NewUser", "asdf", "12345", "xyz", "user88@gmail.com", Language.English, TimeZone.CurrentTimeZone, new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", "2233344", "1234");
            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
            user.IsUserBlocked = new IsUserBlocked(false);
            _persistenceRepository.SaveUpdate(user);
            User receivedUser = _userRepository.GetUserByEmail("User88@Gmail.com");
            Assert.NotNull(receivedUser);
            Assert.AreEqual(user.Username, receivedUser.Username);
            Assert.AreEqual(user.Password, receivedUser.Password);
            Assert.AreEqual(user.PublicKey, receivedUser.PublicKey);
            Assert.AreEqual(user.Language, receivedUser.Language);
            Assert.AreEqual(user.AutoLogout, receivedUser.AutoLogout);
            Assert.AreEqual(user.TimeZone.ToString(), receivedUser.TimeZone.ToString());
            Assert.AreEqual(user.Country, receivedUser.Country);
            Assert.AreEqual(user.State, receivedUser.State);
            Assert.AreEqual(user.PhoneNumber, receivedUser.PhoneNumber);
            Assert.AreEqual(user.Address1, receivedUser.Address1);
            Assert.AreEqual(user.ActivationKey, receivedUser.ActivationKey);
            Assert.AreEqual(user.IsActivationKeyUsed.Value, true);
            Assert.AreEqual(user.IsUserBlocked.Value, false);
        }

        [Test]
        [Category("Integration")]
        public void CreateNewUser_PersistUserAndGetThatUserByActivationKeyFromDatabase_BothUserAreSame()
        {
            User user = new User("NewUser", "asdf", "12345", "xyz", "user88@gmail.com", Language.English, TimeZone.CurrentTimeZone, new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", "2233344", "1234");
            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
            user.IsUserBlocked = new IsUserBlocked(false);
            _persistenceRepository.SaveUpdate(user);
            User receivedUser = _userRepository.GetUserByActivationKey("1234");
            Assert.NotNull(receivedUser);
            Assert.AreEqual(user.Username, receivedUser.Username);
            Assert.AreEqual(user.Password, receivedUser.Password);
            Assert.AreEqual(user.PublicKey, receivedUser.PublicKey);
            Assert.AreEqual(user.Language, receivedUser.Language);
            Assert.AreEqual(user.AutoLogout, receivedUser.AutoLogout);
            Assert.AreEqual(user.TimeZone.ToString(), receivedUser.TimeZone.ToString());
            Assert.AreEqual(user.Country, receivedUser.Country);
            Assert.AreEqual(user.State, receivedUser.State);
            Assert.AreEqual(user.PhoneNumber, receivedUser.PhoneNumber);
            Assert.AreEqual(user.Address1, receivedUser.Address1);
            Assert.AreEqual(user.ActivationKey, receivedUser.ActivationKey);
            Assert.AreEqual(user.IsActivationKeyUsed.Value, true);
            Assert.AreEqual(user.IsUserBlocked.Value, false);
        }

        [Test]
        [Category("Integration")]
        public void CreateNewUser_PersistUserAndGetThatUserByUsernameAndEmailFromDatabase_BothUserAreSame()
        {
            User user = new User("NewUser", "asdf", "12345", "xyz", "user88@gmail.com", Language.English, TimeZone.CurrentTimeZone, new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", "2233344", "1234");
            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
            user.IsUserBlocked = new IsUserBlocked(false);
            _persistenceRepository.SaveUpdate(user);
            User receivedUser = _userRepository.GetUserByEmailAndUserName("NewUser", "user88@gmail.com");
            Assert.NotNull(receivedUser);
            Assert.AreEqual(user.Username, receivedUser.Username);
            Assert.AreEqual(user.Password, receivedUser.Password);
            Assert.AreEqual(user.PublicKey, receivedUser.PublicKey);
            Assert.AreEqual(user.Language, receivedUser.Language);
            Assert.AreEqual(user.AutoLogout, receivedUser.AutoLogout);
            Assert.AreEqual(user.TimeZone.ToString(), receivedUser.TimeZone.ToString());
            Assert.AreEqual(user.Country, receivedUser.Country);
            Assert.AreEqual(user.State, receivedUser.State);
            Assert.AreEqual(user.PhoneNumber, receivedUser.PhoneNumber);
            Assert.AreEqual(user.Address1, receivedUser.Address1);
            Assert.AreEqual(user.ActivationKey, receivedUser.ActivationKey);
            Assert.AreEqual(user.IsActivationKeyUsed.Value, true);
            Assert.AreEqual(user.IsUserBlocked.Value, false);
        }

        [Test]
        [Category("Integration")]
        public void CreateNewUser_ReadUserAndDeleteIt_UserShouldGetDeleted()
        {
            User user = new User("NewUser", "asdf", "12345", "xyz", "user88@gmail.com", Language.English, TimeZone.CurrentTimeZone, new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", "2233344", "1234");
            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
            user.IsUserBlocked = new IsUserBlocked(false);
            _persistenceRepository.SaveUpdate(user);
            User receivedUser = _userRepository.GetUserByUserName("NewUser");
            Assert.NotNull(receivedUser);
            Assert.AreEqual(user.Username, receivedUser.Username);
            Assert.AreEqual(user.Password, receivedUser.Password);
            Assert.AreEqual(user.PublicKey, receivedUser.PublicKey);
            Assert.AreEqual(user.Language, receivedUser.Language);
            Assert.AreEqual(user.AutoLogout, receivedUser.AutoLogout);
            Assert.AreEqual(user.TimeZone.ToString(), receivedUser.TimeZone.ToString());
            Assert.AreEqual(user.Country, receivedUser.Country);
            Assert.AreEqual(user.State, receivedUser.State);
            Assert.AreEqual(user.PhoneNumber, receivedUser.PhoneNumber);
            Assert.AreEqual(user.Address1, receivedUser.Address1);
            Assert.AreEqual(user.ActivationKey, receivedUser.ActivationKey);
            Assert.AreEqual(user.IsActivationKeyUsed.Value, true);
            Assert.AreEqual(user.IsUserBlocked.Value, false);
            //delete the user
            _userRepository.DeleteUser(receivedUser);
            //read user again
            User receivedUser1 = _userRepository.GetUserByUserName("NewUser");
            //assert user is deleted
            Assert.Null(receivedUser1);
        }

        [Test]
        [Category("Integration")]
        public void CreateNewUser_AssignForgotPassword_ReadTheCodeAgain()
        {
            User user = new User("NewUser", "asdf", "12345", "xyz", "user88@gmail.com", Language.English, TimeZone.CurrentTimeZone, new TimeSpan(1, 1, 1, 1), DateTime.Now, "Pakistan", "", "2233344", "1234");
            user.IsActivationKeyUsed = new IsActivationKeyUsed(true);
            user.IsUserBlocked = new IsUserBlocked(false);
            user.AddForgotPasswordCode("123456");
            _persistenceRepository.SaveUpdate(user);
            User receivedUser = _userRepository.GetUserByUserName("NewUser");
            Assert.NotNull(receivedUser);
            Assert.AreEqual(user.Username, receivedUser.Username);
            Assert.AreEqual(user.Password, receivedUser.Password);
            Assert.AreEqual(user.PublicKey, receivedUser.PublicKey);
            Assert.AreEqual(user.Language, receivedUser.Language);
            Assert.AreEqual(user.AutoLogout, receivedUser.AutoLogout);
            Assert.AreEqual(user.TimeZone.ToString(), receivedUser.TimeZone.ToString());
            Assert.AreEqual(user.Country, receivedUser.Country);
            Assert.AreEqual(user.State, receivedUser.State);
            Assert.AreEqual(user.PhoneNumber, receivedUser.PhoneNumber);
            Assert.AreEqual(user.Address1, receivedUser.Address1);
            Assert.AreEqual(user.ActivationKey, receivedUser.ActivationKey);
            Assert.AreEqual(user.IsActivationKeyUsed.Value, true);
            Assert.AreEqual(user.IsUserBlocked.Value, false);
            Assert.AreEqual(user.ForgottenPasswordCodes.Length,receivedUser.ForgottenPasswordCodes.Length);
        }

    }
}
