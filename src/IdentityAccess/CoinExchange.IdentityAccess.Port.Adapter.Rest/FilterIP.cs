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
