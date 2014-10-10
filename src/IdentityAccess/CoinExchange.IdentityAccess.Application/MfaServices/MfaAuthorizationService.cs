using System;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.Services;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.MfaServices
{
    /// <summary>
    /// Provides authorization for Two Factor Authentication if subscribed by the user for the given action
    /// </summary>
    public class MfaAuthorizationService : IMfaAuthorizationService
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IIdentityAccessPersistenceRepository _persistenceRepository = null;
        private IUserRepository _userRepository = null;
        private IMfaCodeSenderService _smsService = null;
        private IMfaCodeSenderService _emailService = null;
        private IMfaCodeGenerationService _codeGenerationService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaAuthorizationService(IIdentityAccessPersistenceRepository persistenceRepository, IUserRepository userRepository,
            IMfaCodeSenderService smsService, IMfaCodeSenderService emailService, IMfaCodeGenerationService codeGenerationService)
        {
            _persistenceRepository = persistenceRepository;
            _userRepository = userRepository;
            _smsService = smsService;
            _emailService = emailService;
            _codeGenerationService = codeGenerationService;
        }

        /// <summary>
        /// Authenticate the user using TFA if it is subscribed for the given action
        /// Returns Tuple: Item1 = Response, Item2 = Error Message
        /// </summary>
        /// <param name="userId"> </param>
        /// <param name="currentAction"></param>
        /// <param name="mfaCode"></param>
        /// <returns></returns>
        public Tuple<bool,string> AuthorizeAccess(int userId, string currentAction, string mfaCode)
        {
            // Get user from repository
            User user = _userRepository.GetUserById(userId);
            if (user != null)
            {
                // Check if the user has asked for the TFA for the current given action
                if (user.CheckMfaSubscriptions(currentAction))
                {
                    // If yes, check if the user contains a TFA code. If he does, verify the code
                    if (!string.IsNullOrEmpty(user.MfaCode))
                    {
                        // If the given code is not null or empty
                        if (!string.IsNullOrEmpty(mfaCode))
                        {
                            // If the given code matches the user's stored mfa code, then return true
                            if (user.VerifyMfaCode(mfaCode))
                            {
                                return new Tuple<bool, string>(true, "Verification Successful");
                            }
                            else
                            {
                                Log.Error(string.Format("MFA code could not be verified: User ID = {0}, Action = {1}", userId,
                                                        currentAction));
                                throw new InvalidOperationException("MFA code could not be verifiedd");
                            }
                        }
                        else
                        {
                            Log.Error(string.Format("MFA code is null: User ID = {0}, Action = {1}", userId, currentAction));
                            throw new NullReferenceException(string.Format("Given MFA code is null: User ID = {0}", userId));
                        }
                    }
                    // Else, send the user a new code, via email or SMS, to whichever the user has subscribed to
                    else
                    {
                        // Generate a new one time pass code
                        string theCode = _codeGenerationService.GenerateCode();
                        // Returns if email is enabled or not, and Email Address/Phone Number respectively
                        Tuple<bool, string> isEmailMfaEnabled = user.IsEmailMfaEnabled();
                        // Assign the MFA code to the user instance
                        user.AssignMfaCode(theCode);
                        // Save user instance with changes
                        _persistenceRepository.SaveUpdate(user);
                        // Get the service to which the user has subscribed to, Email or SMS
                        IMfaCodeSenderService mfaCodeSenderService = GetService(isEmailMfaEnabled.Item1);
                        // Send the user an MFA Code and assign it to the current user
                        mfaCodeSenderService.SendCode(isEmailMfaEnabled.Item2, theCode);
                        return new Tuple<bool, string>(false, "Enter TFA");
                    }
                }
                // If the user has not subscribed TFA for any action, then let the user go ahead without any more checks
                else
                {
                    Log.Debug(string.Format("MFA not enabled: User ID = {0} | Action = {1}. Request will proceed", userId, currentAction));
                    return new Tuple<bool, string>(true, "No MFA subscription enabled");
                }
            }
            // If no user instance is found
            else
            {
                Log.Error(string.Format("No User found during MFA verification: User ID = {0}, Action = {1}", userId, currentAction));
                throw new NullReferenceException(string.Format("No User found for ID = {0}", userId));
            }
        }

        /// <summary>
        /// Gets the corresponding code sender service
        /// </summary>
        /// <param name="emailEnabled"></param>
        private IMfaCodeSenderService GetService(bool emailEnabled)
        {
            if (emailEnabled)
            {
                return _emailService;
            }
            else
            {
                return _smsService;
            }
        }
    }
}
