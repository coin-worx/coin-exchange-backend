using System;
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
            Environment.NewLine, "You requested to reset your passwort BlancRock exchange, please click on the given link" +
                                 "to reset your password", Environment.NewLine, Environment.NewLine);

        private static readonly string WelcomeStart = string.Format("{0} {1} {2} {3} {4}", Environment.NewLine,
            Environment.NewLine, "You have successfully activated your account at BlancRock. We welcome you to our exchange" +
                                 " and are excited to serve you. ", Environment.NewLine, Environment.NewLine);

        private static readonly string PasswordChanged = string.Format("{0} {1} {2} {3} {4}", Environment.NewLine,
            Environment.NewLine, "You have just successfully changed your password. If it wasn't you, please cntact BlancRock" +
                                 " support as soon as possible. ", Environment.NewLine, Environment.NewLine);

        #endregion Private Fields

        #region Pulbic Fields

        public static readonly string Greetings = string.Format("{0} {1} {2} ", "Hi", Environment.NewLine, Environment.NewLine);
        
        public static readonly string ActivationKeySubject = "Activate BlancRock account";

        public static readonly string ForgotUsernameSubject = "Forgot BlancRock Username";

        public static readonly string WelcomeSubject = "Welcome to BlancRock";

        public static readonly string Regards = string.Format("{0} {1} {2}, {3} {4}", System.Environment.NewLine,
            System.Environment.NewLine, "Thanks", System.Environment.NewLine, "BlancRock Team");

        #endregion Public Fields

        /// <summary>
        /// Returns a message that needs to be sent after a user has signed up for BlancRock
        /// </summary>
        /// <param name="activationKey"></param>
        /// <returns></returns>
        public static string GetActivationKeyMessage(string activationKey)
        {
            return string.Format("{0} {1} {2} {3} {4}", Greetings, ActivationKeyEmailStart, activationKey, ActivationKeyEmailEnd, Regards);
        }

        /// <summary>
        /// Forgot Username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetForgotUsernameMessage(string username)
        {
            return string.Format("{0} {1} {2} {3} {4}", Greetings, ForgotUsernameStart, username, ForgotUsernameEnd, Regards);
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
            return string.Format("{0} {1}, {2} {3}", Greetings, username, WelcomeStart, Regards);
        }

        /// <summary>
        /// Gets the message for sending password cahnged email notification to the user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetPasswordChangedEmail(string username)
        {
            return string.Format("{0} {1}, {2} {3}", Greetings, username, PasswordChanged, Regards);
        }
    }
}
