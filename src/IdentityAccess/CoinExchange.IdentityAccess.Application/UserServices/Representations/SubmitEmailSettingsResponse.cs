using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Representations
{
    /// <summary>
    /// Reponse after submitting the response for the email notifications
    /// </summary>
    public class SubmitEmailSettingsResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public SubmitEmailSettingsResponse(bool submissionSuccessful)
        {
            SubmissionSuccessful = submissionSuccessful;
        }

        /// <summary>
        /// If the submission was successful or not
        /// </summary>
        public bool SubmissionSuccessful { get; private set; }
    }
}
