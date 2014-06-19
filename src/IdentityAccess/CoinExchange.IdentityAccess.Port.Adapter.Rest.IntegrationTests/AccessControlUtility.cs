using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.IntegrationTests
{
    /// <summary>
    /// Common functionality for most of the test case needed(register and login)
    /// </summary>
    public class AccessControlUtility
    {
        /// <summary>
        /// Register and login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="applicationContext"></param>
        /// <returns></returns>
        public static UserValidationEssentials RegisterAndLogin(string userName,string email,string password,IApplicationContext applicationContext)
        {
            //register
            RegistrationController registrationController =
                applicationContext["RegistrationController"] as RegistrationController;
            IHttpActionResult httpActionResult = registrationController.Register(new SignUpParam(email, userName, password, "Pakistan",
                TimeZone.CurrentTimeZone, ""));
            OkNegotiatedContentResult<string> okResponseMessage =
                (OkNegotiatedContentResult<string>)httpActionResult;
            string activationKey = okResponseMessage.Content;
            
            //activate account
            UserController userController = applicationContext["UserController"] as UserController;
            httpActionResult = userController.ActivateUser(new UserActivationParam(userName, password, activationKey));
            
            //login
            LoginController loginController = applicationContext["LoginController"] as LoginController;
            httpActionResult = loginController.Login(new LoginParams("user", "123"));
            OkNegotiatedContentResult<UserValidationEssentials> keys =
                (OkNegotiatedContentResult<UserValidationEssentials>)httpActionResult;
            //return keys
            return keys.Content;
        }
    }
}
