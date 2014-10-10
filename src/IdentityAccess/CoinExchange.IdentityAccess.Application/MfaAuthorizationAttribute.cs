using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using CoinExchange.IdentityAccess.Domain.Model.Services;
using Spring.Context.Support;

namespace CoinExchange.IdentityAccess.Application
{
    /// <summary>
    /// Checks user's authorization subscription and acts accordingly
    /// </summary>
    public class MfaAuthorizationAttribute : ActionFilterAttribute
    {
        // Get Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
              (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IMfaAuthorizationService _mfaAuthorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.Filters.ActionFilterAttribute"/> class.
        /// </summary>
        public MfaAuthorizationAttribute()
        {
            _mfaAuthorizationService = (IMfaAuthorizationService)ContextRegistry.GetContext().GetObject("MfaAuthorizationService");
        }

        /// <summary>
        /// Checks Mfa Authorization for a user before the action executes
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (_mfaAuthorizationService.AuthorizeAccess(1, "Deposit", "1234").Item1)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug(string.Format("Mfa Code verification Successful"));
                }
                base.OnActionExecuting(actionContext);
            }
            else
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug(string.Format("Mfa Code verification failed"));
                }
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
