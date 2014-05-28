using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;

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
        /// <returns></returns>
        public bool SendMail(string to, string subject, string content)
        {
            _mailMessage = new MailMessage(_from, to);
            _mailMessage.Subject = subject;
            _mailMessage.Body = content;

            _smtpClient.SendCompleted += SendCompletedCallback;
            _smtpClient.SendAsync(_mailMessage, null);
            return true;
        }

        /// <summary>
        /// Sends the mail that the user should get after signing up for CoinExchange
        /// </summary>
        /// <param name="to"></param>
        /// <param name="activationKey"></param>
        /// <returns></returns>
        public bool SendPostSignUpEmail(string to, string activationKey)
        {
            _mailMessage = new MailMessage(_from, to);
            _mailMessage.Subject = EmailContents.ActivationKeySubject;
            _mailMessage.Body = EmailContents.GetActivationKeyMessage(activationKey);

            _smtpClient.SendAsync(_mailMessage, null);
            return true;
        }

        /// <summary>
        /// Sends the email that the user should get when they request us to remind them of their username
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool SendForgotUsernameEmail(string to, string username)
        {
            _mailMessage = new MailMessage(_from, to);
            _mailMessage.Subject = EmailContents.ForgotUsernameSubject;
            _mailMessage.Body = EmailContents.GetForgotUsernameMessage(username);

            _smtpClient.SendAsync(_mailMessage, null);
            return true;
        }

        /// <summary>
        /// Handles the callback that is called after the mail sending operation is completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
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
