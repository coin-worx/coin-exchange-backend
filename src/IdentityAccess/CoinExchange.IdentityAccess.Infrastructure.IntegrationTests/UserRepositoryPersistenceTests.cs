using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NHibernate.Mapping;
using NUnit.Framework;

namespace CoinExchange.IdentityAccess.Infrastructure.IntegrationTests
{
    [TestFixture]
    public class UserRepositoryPersistenceTests:AbstractConfiguration
    {
        private IPersistenceRepository _persistenceRepository;
        private IUserRepository _userRepository;

        //properties will be injected based on type
        public IUserRepository UserRepository
        {
            set { _userRepository = value; }
        }
        public IPersistenceRepository PersistenceRepository
        {
            set { _persistenceRepository = value; }
        }

        [Test]
        [Category("Integration")]
        public void CreateNewUser_PersistUserAndGetThatUserFromDB_BothUserAreSame()
        {
           User user=new User("Bilal","asdf","12345",new Address("xyz","abc"),"bilal78699",Language.English, TimeZone.CurrentTimeZone,new TimeSpan(1,1,1,1),DateTime.Now,"Pakistan","","2233344","1234"); 
           _persistenceRepository.SaveUpdate(user);
            User receivedUser = _userRepository.GetUserByUserName("Bilal");
            Assert.NotNull(receivedUser);
            Assert.AreEqual(user.Username,receivedUser.Username);
            Assert.AreEqual(user.Password, receivedUser.Password);
            Assert.AreEqual(user.PublicKey, receivedUser.PublicKey);
            Assert.AreEqual(user.Language, receivedUser.Language);
            Assert.AreEqual(user.AutoLogout, receivedUser.AutoLogout);
            Assert.AreEqual(user.TimeZone,receivedUser.TimeZone);
            Assert.AreEqual(user.Country, receivedUser.Country);
            Assert.AreEqual(user.State, receivedUser.State);
            Assert.AreEqual(user.PhoneNumber, receivedUser.PhoneNumber);
            Assert.AreEqual(user.Address,receivedUser.Address);
            Assert.AreEqual(user.ActivationKey, receivedUser.ActivationKey);
        }
    }
}
