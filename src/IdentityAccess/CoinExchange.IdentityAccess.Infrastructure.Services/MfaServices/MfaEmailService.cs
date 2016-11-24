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
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;

namespace CoinExchange.IdentityAccess.Infrastructure.Services.MfaServices
{
    /// <summary>
    /// Service to send the MFA Code ia email to the user
    /// </summary>
    public class MfaEmailService : IMfaCodeSenderService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SmtpClient _smtpClient = null;
        private string _from = null;
        private MailMessage _mailMessage = null;

        /// <summary>
        /// Checks whether a mail sending async operation is in process
        /// </summary>
        private bool _sendingInProgress = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaEmailService()
        {
            _smtpClient = new SmtpClient();
            _from = ConfigurationManager.AppSettings.Get("MfaEmailAddress");
            _smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MfaEmailPort"));
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.Host = ConfigurationManager.AppSettings.Get("MfaEmailHost");
            _smtpClient.Credentials = new NetworkCredential(_from, ConfigurationManager.AppSettings.Get("MfaEmailPassword"));
            _smtpClient.EnableSsl = true;
            _smtpClient.SendCompleted += SendCompletedCallback;
        }

        /// <summary>
        /// Send the code through email
        /// </summary>
        /// <param name="to"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool SendCode(string to, string code)
        {
            _mailMessage = new MailMessage(_from, to);
            _mailMessage.Subject = "Crypgo TFA Security Code";
            _mailMessage.Body = "Your TFA Security Code is: " + code;

            // Until the previous email sending operation is in process, wait otherwise exception will be thrown.
            while (_sendingInProgress)
            {
                Thread.Sleep(500);
            }

            try
            {
                Log.Debug(string.Format("Sending Email for TFA code to email: {0}", to));
                _smtpClient.SendAsync(_mailMessage, null);
                Log.Debug(string.Format("Email sent for TFA code to email: {0}", to));
                return true;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Error while sending Email for TFA code to email: {0} | Exception = {1}", to, e));
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
                Log.Info(string.Format("{0} {1}", "MFA Code Email cancelled for: ", _mailMessage.To.ToString()));
            }
            if (e.Error != null)
            {
                Log.Info(string.Format("{0} {1}", "Error while sending MFA Code email to: ", _mailMessage.To.ToString()));
            }
            // Mail sent
            else
            {
                Log.Info(string.Format("{0} {1}", "MFA Code Email Sent to: ", _mailMessage.To.ToString()));
            }
        }
    }
}
