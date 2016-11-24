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

namespace CoinExchange.IdentityAccess.Infrastructure.Services.Email
{
    /// <summary>
    /// Contains text to be used for different email bodies
    /// </summary>
    public class EmailContents
    {
        #region Private Fields

        private static readonly string ActivationKeyEmailStart = string.Format("{0} {1} {2}", "You have just signed up for an account at BlancRock. Please" +
                                                           " activate your account by entering the following Activation Key along with " +
                                                           "your username and password on the Activate Account page to activate your " +
                                                           "account.", System.Environment.NewLine, System.Environment.NewLine);

        private static readonly string ActivationKeyEmailEnd = string.Format("{0} {1} {2}", System.Environment.NewLine,
            System.Environment.NewLine, "If you haven't signed up using this email, please ignore this message.");

        private static readonly string ForgotUsernameStart = string.Format("{0} {1} {2}", "You just requested BlancRock Exchange to remind you" +
            " of your username that you have forgotten. You Username is: ", Environment.NewLine, Environment.NewLine);

        private static readonly string ForgotUsernameEnd = string.Format("{0} {1} {2}", Environment.NewLine,
            Environment.NewLine, "Enjoy!!!");

        private static readonly string ForgotPasswordStart = string.Format("{0} {1} {2} {3} {4}", Environment.NewLine,
            Environment.NewLine, "You requested to reset your password at BlancRock exchange, please click on the given link" +
                                 "to reset your password", Environment.NewLine, Environment.NewLine);

        private static readonly string WelcomeStart = string.Format("{0} {1} {2} {3} {4}", Environment.NewLine,
            Environment.NewLine, "You have successfully activated your account at BlancRock. We welcome you to our exchange" +
                                 " and are excited to serve you. ", Environment.NewLine, Environment.NewLine);

        private static readonly string PasswordChanged = string.Format("{0} {1} {2}", "You have just successfully changed your password. If it wasn't you, please cntact BlancRock" +
                                 " support as soon as possible. ", Environment.NewLine, Environment.NewLine);

        private static readonly string ReActivation = string.Format("{0} {1} {2}", Environment.NewLine,
            Environment.NewLine, "You have just tried to re-activate your account, and as the policy guidelines state, an" +
                                 " account cannot be activated more than once. If it wasn't you, please contact the BlancRock" +
                                 " support as soon as possible because some is trying to get access into your account. ");

        private static readonly string CancelActivation = string.Format("{0} {1} {2}", Environment.NewLine,
            Environment.NewLine, "You have jsut cancelled your activation for lancRock account. Please feel to register again" +
                                 " or to query our support anytime you decide to change your mind.  ");

        #endregion Private Fields

        #region Pulbic Fields

        public static readonly string Greetings = string.Format("{0} ", "Hi");
        
        public static readonly string ActivationKeySubject = "Activate BlancRock account";

        public static readonly string ForgotUsernameSubject = "Forgot BlancRock Username";

        public static readonly string PasswordChangedSubject = "BlancRock Password Changed";

        public static readonly string WelcomeSubject = "Welcome to BlancRock";

        public static readonly string ReactivationNotificationSubject = "Account Re-activation attempted.";

        public static readonly string CancelActivationSubject = "Account Activation Cancelled.";

        public static readonly string Regards = string.Format("{0} {1} {2}, {3} {4}", System.Environment.NewLine,
            System.Environment.NewLine, "Thanks", System.Environment.NewLine, "BlancRock Team");

        #endregion Public Fields

        /// <summary>
        /// Returns a message that needs to be sent after a user has signed up for BlancRock
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="activationKey"></param>
        /// <returns></returns>
        public static string GetActivationKeyMessage(string username, string activationKey)
        {
            return string.Format("{0} {1}, {2} {3} {4} {5} {6} {7}", Greetings, username, Environment.NewLine, Environment.NewLine, ActivationKeyEmailStart, activationKey, ActivationKeyEmailEnd, Regards);
        }

        /// <summary>
        /// Forgot Username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetForgotUsernameMessage(string username)
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6}", Greetings, Environment.NewLine, Environment.NewLine, ForgotUsernameStart, username, ForgotUsernameEnd, Regards);
        }

        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <param name="passwordResetLink"> </param>
        /// <returns></returns>
        public static string GetForgotPasswordMessage(string passwordResetLink)
        {
            return string.Format("{0} {1} {2} {3}", Greetings, ForgotPasswordStart, passwordResetLink, Regards);
        }

        /// <summary>
        /// Gets the massage for sending a welcome mail to the user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetWelcomEmailmessage(string username)
        {
            return string.Format("{0} {1}, {2} {3} {4} {5} ", Greetings, username, Environment.NewLine, Environment.NewLine, WelcomeStart, Regards);
        }

        /// <summary>
        /// Gets the message for sending password cahnged email notification to the user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetPasswordChangedEmail(string username)
        {
            return string.Format("{0} {1}, {2} {3} {4} {5}", Greetings, username, Environment.NewLine, Environment.NewLine, PasswordChanged, Regards);
        }

        /// <summary>
        /// Gets the message when an account is attempted ot be re-activated
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetReactivationNotificationEmail(string username)
        {
            return string.Format("{0} {1}, {2} {3}", Greetings, username, ReActivation, Regards);
        }

        /// <summary>
        /// Gets the email when a potential user cancels their account activation
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetCancelActivationEmail(string username)
        {
            return string.Format("{0} {1}, {2} {3}", Greetings, username, CancelActivation, Regards);
        }
    }
}
