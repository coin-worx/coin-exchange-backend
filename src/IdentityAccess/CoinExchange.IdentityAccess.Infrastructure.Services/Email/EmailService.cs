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
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace CoinExchange.IdentityAccess.Infrastructure.Services.Email
{
    /// <summary>
    /// Email Service
    /// </summary>
    public class EmailService : IEmailService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SmtpClient _smtpClient = new SmtpClient();
        private string _from;
        private MailMessage _mailMessage = null;

        /// <summary>
        /// Checks whether a mail sending async operation is in process
        /// </summary>
        private bool _sendingInProgress = false;

        /// <summary>
        /// Initializes the Email service
        /// </summary>
        public EmailService(string host, int port, string from, string password)
        {
            _from = from;
            _smtpClient.Port = port;
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.Host = host;
            _smtpClient.Credentials = new NetworkCredential(from, password);
            _smtpClient.EnableSsl = true;
            _smtpClient.SendCompleted += SendCompletedCallback;
        }

        #region Methods

        /// <summary>
        /// Sends the email to the given address, with the given subject and the given Content
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        public bool SendMail(string to, string subject, string content, bool adminEmailsAllowed)
        {
            if (adminEmailsAllowed)
            {
                _mailMessage = new MailMessage(_from, to);
                _mailMessage.Subject = subject;
                _mailMessage.Body = content;

                // Until the previous email sending operation is in process, wait otherwise exception will be thrown.
                while (_sendingInProgress)
                {
                    Thread.Sleep(500);
                }
                _smtpClient.SendAsync(_mailMessage, null);
                _sendingInProgress = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends the mail that the user should get after signing up for CoinExchange
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"> </param>
        /// <param name="activationKey"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        public bool SendPostSignUpEmail(string to, string username, string activationKey, bool adminEmailsAllowed)
        {
            if (adminEmailsAllowed)
            {
                _mailMessage = new MailMessage(_from, to);
                _mailMessage.Subject = EmailContents.ActivationKeySubject;
                _mailMessage.Body = EmailContents.GetActivationKeyMessage(username, activationKey);

                // Until the previous email sending operation is in process, wait otherwise exception will be thrown.
                while (_sendingInProgress)
                {
                    Thread.Sleep(500);
                }
                _smtpClient.SendAsync(_mailMessage, null);
                _sendingInProgress = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends the email that the user should get when they request us to remind them of their username
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        public bool SendForgotUsernameEmail(string to, string username, bool adminEmailsAllowed)
        {
            if (adminEmailsAllowed)
            {
                _mailMessage = new MailMessage(_from, to);
                _mailMessage.Subject = EmailContents.ForgotUsernameSubject;
                _mailMessage.Body = EmailContents.GetForgotUsernameMessage(username);

                // Until the previous email sending operation is in process, wait otherwise exception will be thrown.
                while (_sendingInProgress)
                {
                    Thread.Sleep(500);
                }
                _smtpClient.SendAsync(_mailMessage, null);
                _sendingInProgress = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Send Welcome EMail to the specified user
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        public bool SendWelcomeEmail(string to, string username, bool adminEmailsAllowed)
        {
            if (adminEmailsAllowed)
            {
                _mailMessage = new MailMessage(_from, to);
                _mailMessage.Subject = EmailContents.WelcomeSubject;
                _mailMessage.Body = EmailContents.GetWelcomEmailmessage(username);

                // Until the previous email sending operation is in process, wait otherwise exception will be thrown.
                while (_sendingInProgress)
                {
                    Thread.Sleep(500);
                }
                _smtpClient.SendAsync(_mailMessage, null);
                _sendingInProgress = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Send email to the user to notify them that they just changed their password
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        public bool SendPasswordChangedEmail(string to, string username, bool adminEmailsAllowed)
        {
            if (adminEmailsAllowed)
            {
                _mailMessage = new MailMessage(_from, to);
                _mailMessage.Subject = EmailContents.PasswordChangedSubject;
                _mailMessage.Body = EmailContents.GetPasswordChangedEmail(username);

                // Until the previous email sending operation is in process, wait otherwise exception will be thrown.
                while (_sendingInProgress)
                {
                    Thread.Sleep(500);
                }
                _smtpClient.SendAsync(_mailMessage, null);
                _sendingInProgress = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends an email to user specifying that their account was tried to be re-activated
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        public bool SendReactivaitonNotificationEmail(string to, string username, bool adminEmailsAllowed)
        {
            if (adminEmailsAllowed)
            {
                _mailMessage = new MailMessage(_from, to);
                _mailMessage.Subject = EmailContents.ReactivationNotificationSubject;
                _mailMessage.Body = EmailContents.GetReactivationNotificationEmail(username);

                _smtpClient.SendAsync(_mailMessage, null);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends an email to the potential user that specifies that they have just cancelled their activation
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="adminEmailsAllowed"> </param>
        /// <param name="newsletterEmailsAllowed"> </param>
        /// <returns></returns>
        public bool SendCancelActivationEmail(string to, string username, bool adminEmailsAllowed)
        {
            if (adminEmailsAllowed)
            {
                _mailMessage = new MailMessage(_from, to);
                _mailMessage.Subject = EmailContents.CancelActivationSubject;
                _mailMessage.Body = EmailContents.GetCancelActivationEmail(username);

                // Until the previous email sending operation is in process, wait otherwise exception will be thrown.
                while (_sendingInProgress)
                {
                    Thread.Sleep(500);
                }
                _smtpClient.SendAsync(_mailMessage, null);
                _sendingInProgress = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the callback that is called after the mail sending operation is completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            _sendingInProgress = false;
            if (e.Cancelled)
            {
                Log.Info(string.Format("{0} {1}","Email cancelled for: ", _mailMessage.To.ToString()));
            }
            if (e.Error != null)
            {
                Log.Info(string.Format("{0} {1}", "Error while sending email to: ", _mailMessage.To.ToString()));
            }
            // Mail sent
            else
            {
                Log.Info(string.Format("{0} {1}", "Email Sent to: ", _mailMessage.To.ToString()));
            }
        }

        #endregion Properties

        #region Properties

        /// <summary>
        /// Instance of the SMTP Client
        /// </summary>
        public SmtpClient SmtpClient { get; internal set; }

        /// <summary>
        /// The address from which the email will be sent
        /// </summary>
        public string FromAddress
        {
            get { return _from; }
        }

        #endregion Properties
    }
}
