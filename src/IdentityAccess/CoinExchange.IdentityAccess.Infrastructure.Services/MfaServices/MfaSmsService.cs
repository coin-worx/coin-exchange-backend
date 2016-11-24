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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.Services;
using Twilio;

namespace CoinExchange.IdentityAccess.Infrastructure.Services.MfaServices
{
    /// <summary>
    /// Service to send the MFA code to the user via SMS
    /// </summary>
    public class MfaSmsService : IMfaCodeSenderService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private TwilioRestClient _twilio;

        private string AccountSid = ConfigurationManager.AppSettings.Get("TwilioSID");
        private string AuthToken = ConfigurationManager.AppSettings.Get("TwilioAuthToken");
        private string PhoneNumber = ConfigurationManager.AppSettings.Get("TwilioPhoneNumber");

        public event Action<Message> SmsCallback;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MfaSmsService()
        {
            _twilio = new TwilioRestClient(AccountSid, AuthToken);
            SmsCallback += OnSmsCallback;
        }

        /// <summary>
        /// Sends the codee to the user asynchronously
        /// </summary>
        /// <param name="address"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool SendCode(string address, string code)
        {
            try
            {
                Log.Debug(string.Format("Sending TFA code to number: {0}", address));

                _twilio.SendMessage(PhoneNumber, address, code, SmsCallback);

                Log.Debug(string.Format("TFA code sent to number: {0}", address));
                return true;
            }
            catch (Exception exception)
            {
                Log.Debug(string.Format("Error while sending TFA code to number: {0}. Exception = {1}", address, exception));
            }
            return false;
        }

        /// <summary>
        /// Callback for every sent sms
        /// </summary>
        /// <param name="message"></param>
        public void OnSmsCallback(Message message)
        {
            Log.Debug(string.Format("Sms recevied to number: {0}", message.To));
        }
    }
}
