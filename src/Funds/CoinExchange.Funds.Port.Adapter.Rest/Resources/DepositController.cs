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
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Services;
using CoinExchange.Funds.Application.BalanceService.Representations;
using CoinExchange.Funds.Application.DepositServices;
using CoinExchange.Funds.Application.DepositServices.Commands;
using CoinExchange.Funds.Application.DepositServices.Representations;
using CoinExchange.Funds.Port.Adapter.Rest.DTOs.Deposit;
using CoinExchange.IdentityAccess.Application;

namespace CoinExchange.Funds.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Deposit Controller
    /// </summary>
    [RoutePrefix("v1")]
    public class DepositController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IDepositApplicationService _depositApplicationService;
        private IApiKeyInfoAccess _apiKeyInfoAccess;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="depositApplicationService"></param>
        /// <param name="apiKeyInfoAccess"></param>
        public DepositController(IDepositApplicationService depositApplicationService, IApiKeyInfoAccess apiKeyInfoAccess)
        {
            _depositApplicationService = depositApplicationService;
            _apiKeyInfoAccess = apiKeyInfoAccess;
        }

        [Route("funds/getrecentdeposits")]
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(List<DepositRepresentation>))]
        public IHttpActionResult GetRecentDeposits([FromBody]GetRecentDepositParams getRecentDepositParams)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Get Recent Deposits call: Currency = {0}",
                    getRecentDepositParams.Currency));
            }
            try
            {
                // Get api key from header
                var headers = Request.Headers;
                string apikey = "";
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    apikey = auth[0];
                }
                if (log.IsDebugEnabled)
                {
                    log.Debug(string.Format("Get Recent Deposits Call: ApiKey = {0}", apikey));
                }
                if (getRecentDepositParams != null && !string.IsNullOrEmpty(getRecentDepositParams.Currency))
                {
                    int accountId = _apiKeyInfoAccess.GetUserIdFromApiKey(apikey);
                    return Ok(_depositApplicationService.GetRecentDeposits(getRecentDepositParams.Currency, accountId));
                }
                return BadRequest("Currency is not provided.");
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Get Recent Deposits Call Error: {0}", exception));
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Call to generate a new address
        /// </summary>
        /// <param name="generateAddressParams"> </param>
        /// <returns></returns>
        [Route("funds/createdepositaddress")]
        [Authorize]
        [HttpPost]
        [MfaAuthorization(MfaConstants.Deposit)]
        [ResponseType(typeof(DepositAddressRepresentation))]
        public IHttpActionResult CreateDepositAddress([FromBody]GenerateAddressParams generateAddressParams)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Deposit Address Generation call: Currency = {0} | Account ID = {1}", 
                    generateAddressParams.Currency, generateAddressParams.AccountId));
            }
            try
            {
                //get api key from header
                var headers = Request.Headers;
                string apikey = "";
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    apikey = auth[0];
                }
                if (log.IsDebugEnabled)
                {
                    log.Debug(string.Format("Generate Deposit Address Call: ApiKey = {0}", apikey));
                }
                if (generateAddressParams != null && !string.IsNullOrEmpty(generateAddressParams.Currency))
                {
                    int accountId = _apiKeyInfoAccess.GetUserIdFromApiKey(apikey);
                    return Ok(_depositApplicationService.GenarateNewAddress(new GenerateNewAddressCommand(
                        accountId, generateAddressParams.Currency)));
                }
                return BadRequest("Currency is not provided.");
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Create New Address Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (NullReferenceException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Create New Address Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InstanceNotFoundException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Create New Address Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Generate Deposit Address Call Error: {0}", exception));
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Call get all the deposit addresses
        /// </summary>
        /// <returns></returns>
        [Route("funds/getdepositaddresses")]
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(IList<DepositAddressRepresentation>))]
        public IHttpActionResult GetDepositAddresses([FromBody]GetDepositAddressesParams depositAddressesParams)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Get Deposit Addresses call"));
            }
            try
            {
                //get api key from header
                var headers = Request.Headers;
                string apikey = "";
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    apikey = auth[0];
                }
                if (log.IsDebugEnabled)
                {
                    log.Debug(string.Format("Get Deposit Addresses Call: ApiKey = {0}", apikey));
                }
                if (!string.IsNullOrEmpty(apikey))
                {
                    int accountId = _apiKeyInfoAccess.GetUserIdFromApiKey(apikey);
                    return Ok(_depositApplicationService.GetAddressesForAccount(accountId, depositAddressesParams.Currency));
                }
                return BadRequest("API key not found.");
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Generate Deposit Address Call Error: {0}", exception));
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Call to get Deposit Limits
        /// </summary>
        /// <returns></returns>
        [Route("funds/getdepositlimits")]
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(DepositLimitThresholdsRepresentation))]
        public IHttpActionResult GetDepositLimits([FromBody]GetDepositLimitsParams getDepositLimitsParams)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Get Deposit Limits call: Currency = {0}", getDepositLimitsParams.Currency));
            }
            try
            {
                // Get api key from header
                var headers = Request.Headers;
                string apikey = "";
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    apikey = auth[0];
                }
                if (log.IsDebugEnabled)
                {
                    log.Debug(string.Format("Get Deposit Limits Call: ApiKey = {0}", apikey));
                }
                if (!string.IsNullOrEmpty(apikey) && !string.IsNullOrEmpty(getDepositLimitsParams.Currency))
                {
                    int accountId = _apiKeyInfoAccess.GetUserIdFromApiKey(apikey);
                    return Ok(_depositApplicationService.GetThresholdLimits(accountId, getDepositLimitsParams.Currency));
                }
                return BadRequest("Currency is not provided or API key not found with request");
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Get Deposit Limits Call Error: {0}", exception));
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Call to get Deposit Limits
        /// </summary>
        /// <returns></returns>
        [Route("funds/makedeposit")]
        [Authorize]
        [HttpPost]
        [MfaAuthorization(MfaConstants.Deposit)]
        [ResponseType(typeof(bool))]
        public IHttpActionResult MakeDeposit([FromBody]MakeDepositParams makeDepositParams)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Make Deposit call: Currency = {0}", makeDepositParams.Currency));
            }
            try
            {
                // Get api key from header
                var headers = Request.Headers;
                string apikey = "";
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    apikey = auth[0];
                }
                if (log.IsDebugEnabled)
                {
                    log.Debug(string.Format("Make Deposit Call: ApiKey = {0}", apikey));
                }
                if (!string.IsNullOrEmpty(apikey) && !string.IsNullOrEmpty(makeDepositParams.Currency))
                {
                    int accountId = _apiKeyInfoAccess.GetUserIdFromApiKey(apikey);
                    return Ok(_depositApplicationService.MakeDeposit(new MakeDepositCommand(
                        accountId, makeDepositParams.Currency, makeDepositParams.Amount, makeDepositParams.IsCryptoCurrency)));
                }
                return BadRequest("Currency is not provided or API key not found with request");
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Make Deposit Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (NullReferenceException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Make Deposit Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InstanceNotFoundException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Make Deposit Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Make Deposit Call Error: {0}", exception));
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Call to get Deposit Limits
        /// </summary>
        /// <returns></returns>
        [Route("funds/getDepositTierLimits")]
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(DepositTierLimitRepresentation))]
        public IHttpActionResult GetDepositTierLimits()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Get Deposit Tier Limits call"));
            }
            try
            {
                // Get api key from header
                var headers = Request.Headers;
                string apikey = "";
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    apikey = auth[0];
                }
                if (log.IsDebugEnabled)
                {
                    log.Debug(string.Format("Get Deposit Tier Limits Call: ApiKey = {0}", apikey));
                }
                if (!string.IsNullOrEmpty(apikey))
                {
                    return Ok(_depositApplicationService.GetDepositTiersLimits());
                }
                return BadRequest("Currency is not provided or API key not found with request");
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Make Deposit Call Error: {0}", exception));
                }
                return InternalServerError();
            }
        }
    }
}
