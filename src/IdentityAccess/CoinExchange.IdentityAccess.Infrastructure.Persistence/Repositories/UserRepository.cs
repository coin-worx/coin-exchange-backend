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
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using NHibernate.Criterion;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.IdentityAccess.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// User repository implementation
    /// </summary>
    [Repository]
    public class UserRepository : NHibernateSessionFactory, IUserRepository
    {
        [Transaction(ReadOnly = true)]
        public User GetUserByUserName(string username)
        {
            return CurrentSession.QueryOver<User>().Where(x => x.Username == username && x.Deleted == false).SingleOrDefault();
        }

        [Transaction(ReadOnly = true)]
        public User GetUserByActivationKey(string activationKey)
        {
            return CurrentSession.QueryOver<User>().Where(x => x.ActivationKey == activationKey&&x.Deleted==false).SingleOrDefault();
        }

        [Transaction(ReadOnly = true)]
        public User GetUserByEmail(string email)
        {
            return CurrentSession.QueryOver<User>().Where(x => x.Email == email&&x.Deleted==false).SingleOrDefault();
        }

        [Transaction(ReadOnly = true)]
        public User GetUserByEmailAndUserName(string username, string email)
        {
            return CurrentSession.QueryOver<User>().Where(x => x.Username == username && x.Email==email && x.Deleted==false).SingleOrDefault();
        }

        // ToDo For Bilal: Please implement this method and return the user. ALso create the Hibernate mapping for 
        // User.ForgotPasswordCode
        public User GetUserByForgotPasswordCode(string forgotPasswordCode)
        {
            throw new NotImplementedException();
        }

        [Transaction(ReadOnly = false)]
        public bool DeleteUser(User user)
        {
            user.Deleted = true;
            CurrentSession.SaveOrUpdate(user);
            return true;
        }

        [Transaction(ReadOnly = true)]
        public User GetUserById(int id)
        {
            User user=CurrentSession.Get<User>(id);
            if (user != null)
            {
                if (user.Deleted)
                {
                    throw new ArgumentException("The user doesnot exist or have been deleted");
                }
                return user;
            }
            return null;
        }
    }
}
