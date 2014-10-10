using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest
{
    /// <summary>
    /// IP address filter
    /// </summary>
    public class FilterIP:ActionFilterAttribute
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
              (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FilterIP()
        {

        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string ipAddress = HttpContext.Current.Request.UserHostAddress;
            
            if (IsIpAllowed(ipAddress))
            {
                //seems ip address is valid
                base.OnActionExecuting(actionContext);
                if (log.IsDebugEnabled)
                {
                    log.Debug("Accepted IpAddress : "+ipAddress);
                }
            }
            else
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Rejected IpAddress : " + ipAddress);
                }
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// check for available ip address
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        private bool IsIpAllowed(string ipaddress)
        {
            //read all allowed ipaddresses from web.config file
            string allowedAddresses =Convert.ToString(ConfigurationManager.AppSettings["AuthorizeIPAddresses"]);
            string[] validIpAddresses = allowedAddresses.Trim().Split(',');
            foreach (var validIpAddress in validIpAddresses)
            {
                if (validIpAddress.Trim().Equals(ipaddress))
                    return true;
            }
            return false;
        }
    }
}
