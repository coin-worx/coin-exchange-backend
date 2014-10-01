using System;
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
