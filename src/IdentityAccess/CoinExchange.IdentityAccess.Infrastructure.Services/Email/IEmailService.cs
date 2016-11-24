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
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Infrastructure.Services.Email
{
    /// <summary>
    /// Interface for the Email Service using SMTP
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends Email to the given recipient
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"> </param>
        /// <param name="content"> </param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        bool SendMail(string to, string subject, string content, bool adminEmailsAllowed);

        /// <summary>
        /// Send activation key after signup
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"> </param>
        /// <param name="activationKey"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        bool SendPostSignUpEmail(string to, string username, string activationKey, bool adminEmailsAllowed);

        /// <summary>
        /// Sends the email that the user should get when they request us to remind them of their username
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        bool SendForgotUsernameEmail(string to, string username, bool adminEmailsAllowed);

        /// <summary>
        /// Sends Welcome Email
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        bool SendWelcomeEmail(string to, string username, bool adminEmailsAllowed);

        /// <summary>
        /// Email the user that they just changed password
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        bool SendPasswordChangedEmail(string to, string username, bool adminEmailsAllowed);

        /// <summary>
        /// Sends an email to user specifying that their account was tried to be re-activated
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        bool SendReactivaitonNotificationEmail(string to, string username, bool adminEmailsAllowed);

        /// <summary>
        /// Sends an email to the potential user that specifies that they have just cancelled their activation
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        bool SendCancelActivationEmail(string to, string username, bool adminEmailsAllowed);

        /// <summary>
        /// Instance of the SMTP Client
        /// </summary>
        SmtpClient SmtpClient { get; }

        /// <summary>
        /// The address from which the Email will be sent
        /// </summary>
        string FromAddress { get; }
    }
}
