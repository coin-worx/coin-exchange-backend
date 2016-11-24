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

namespace CoinExchange.IdentityAccess.Application.Tests
{
    /// <summary>
    /// Mocks User Repository
    /// </summary>
    public class MockUserRepository : IUserRepository
    {
        private List<User> _userList = new List<User>();
        private int _userCounter = 0;

        /// <summary>
        /// Get the User by Username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetUserByUserName(string username)
        {
            foreach (var user in _userList)
            {
                if (user.Username.Equals(username))
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// Get user by activation key
        /// </summary>
        /// <param name="activationKey"></param>
        /// <returns></returns>
        public User GetUserByActivationKey(string activationKey)
        {
            foreach (var user in _userList)
            {
                if (user.ActivationKey == activationKey)
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// Get by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmail(string email)
        {
            foreach (var user in _userList)
            {
                if (user.Email == email)
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// Get by email and username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmailAndUserName(string username, string email)
        {
            throw new NotImplementedException();
        }

        public User GetUserByForgotPasswordCode(string forgotPasswordCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the User to the User collection only in this Mock implementation
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddUser(User user)
        {
            ++_userCounter;
            _userList.Add(user);
            return true;
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteUser(User user)
        {
            _userList.Remove(user);
            return true;
        }

        public User GetUserById(int id)
        {
            foreach (var user in _userList)
            {
                if (user.Id == id)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
