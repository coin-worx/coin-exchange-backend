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
using CoinExchange.Common.Services;
using CoinExchange.Funds.Application.BalanceService.Representations;
using CoinExchange.Funds.Application.LedgerServices;
using CoinExchange.Funds.Application.LedgerServices.Representations;
using CoinExchange.Funds.Application.WithdrawServices;
using CoinExchange.Funds.Port.Adapter.Rest.DTOs.Ledgers;

namespace CoinExchange.Funds.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Ledger Controller
    /// </summary>
    [RoutePrefix("v1")]
    public class LedgerController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ILedgerQueryService _ledgerQueryService;
        private IApiKeyInfoAccess _apiKeyInfoAccess;

        /// <summary>
        /// Defautl Constructor
        /// </summary>
        /// <param name="ledgerQueryService"></param>
        /// <param name="apiKeyInfoAccess"></param>
        public LedgerController(ILedgerQueryService ledgerQueryService, IApiKeyInfoAccess apiKeyInfoAccess)
        {
            _ledgerQueryService = ledgerQueryService;
            _apiKeyInfoAccess = apiKeyInfoAccess;
        }

        /// <summary>
        /// Get all the ledgers for the provided currency for this user
        /// </summary>
        /// <param name="getLedgersParams"></param>
        /// <returns></returns>
        [Route("funds/getledgers")]
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(IList<LedgerRepresentation>))]
        public IHttpActionResult GetLedgersForCurrency([FromBody]GetLedgersParams getLedgersParams)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Get Ledgers for a Currency call: Currency = {0}", getLedgersParams.Currency));
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
                    log.Debug(string.Format("Get Ledgers for a Currency Call: ApiKey = {0}", apikey));
                }
                if (!string.IsNullOrEmpty(getLedgersParams.Currency) && !string.IsNullOrEmpty(apikey))
                {
                    int accountId = _apiKeyInfoAccess.GetUserIdFromApiKey(apikey);
                    return Ok(_ledgerQueryService.GetLedgersForCurrency(accountId, getLedgersParams.Currency));
                }
                return BadRequest("Currency is not provided.");
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Get Ledgers for a Currency Call Error: {0}", exception));
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Get all the ledgers for the provided currency for this user
        /// </summary>
        /// <returns></returns>
        [Route("funds/getallledgers")]
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(IList<LedgerRepresentation>))]
        public IHttpActionResult GetAllLedgers()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Get All Ledgers call"));
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
                    log.Debug(string.Format("Get All Ledgers Call: ApiKey = {0}", apikey));
                }
                if (!string.IsNullOrEmpty(apikey))
                {
                    int accountId = _apiKeyInfoAccess.GetUserIdFromApiKey(apikey);
                    return Ok(_ledgerQueryService.GetAllLedgers(accountId));
                }
                throw new InstanceNotFoundException("No API key found.");
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Get All Ledgers Call Error: {0}", exception));
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Get all the ledgers for the provided currency for this user
        /// </summary>
        /// <param name="getLedgersDetailsParams"></param>
        /// <returns></returns>
        [Route("funds/getledgerdetails")]
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(LedgerRepresentation))]
        public IHttpActionResult GetLedgerDetails([FromBody]GetLedgerDetailsParams getLedgersDetailsParams)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Get Ledger Details call: LedgerId = {0}", getLedgersDetailsParams.LedgerId));
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
                    log.Debug(string.Format("Get Ledger Details Call: ApiKey = {0}", apikey));
                }
                if (!string.IsNullOrEmpty(getLedgersDetailsParams.LedgerId) && !string.IsNullOrEmpty(apikey))
                {
                    return Ok(_ledgerQueryService.GetLedgerDetails(getLedgersDetailsParams.LedgerId));
                }
                return BadRequest("Ledger ID is not provided.");
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(string.Format("Get Ledger Details Call Error: {0}", exception));
                }
                return InternalServerError();
            }
        }
    }
}
