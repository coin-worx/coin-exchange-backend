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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using CoinExchange.Common.Utility;
using CoinExchange.IdentityAccess.Application.SecurityKeysServices.Representations;
using CoinExchange.IdentityAccess.Application.UserServices;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;
using CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Deals with user tier levels status fucntionality
    /// </summary>
    [RoutePrefix("v1")]
    public class TierController:ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
              (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string ServerUploadFolder = "C:\\Temp";
        private IUserTierLevelApplicationService _userTierLevelApplicationService;

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="userTierLevelApplicationService"></param>
        public TierController(IUserTierLevelApplicationService userTierLevelApplicationService)
        {
            _userTierLevelApplicationService = userTierLevelApplicationService;
        }

        /// <summary>
        /// apply for tier 1 verification
        /// </summary>
        /// <param name="param"> </param>
        /// <returns></returns>
        [HttpPost]
        [Route("private/user/applyfortier1")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(bool))]
        public IHttpActionResult GetVerifyForTier1([FromBody]Tier1Param param)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetVerifyForTier1 Call Recevied, parameters:" + param);
                }
                _userTierLevelApplicationService.ApplyForTier1Verification(new VerifyTier1Command(param.PhoneNumber,
                    HeaderParamUtility.GetApikey(Request),param.FullName,Convert.ToDateTime(param.DateOfBirth)));
                return Ok();
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier1 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier1 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier1 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier1 Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// apply for tier 2 verification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("private/user/applyfortier2")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(void))]
        public IHttpActionResult GetVerifyForTier2([FromBody]Tier2Param param)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetVerifyForTier2 Call Recevied, parameters:"+param);
                }
                _userTierLevelApplicationService.ApplyForTier2Verification(
                    new VerifyTier2Command(HeaderParamUtility.GetApikey(Request), param.AddressLine1, param.AddressLine2,
                        param.AddressLine3, param.State, param.City, param.ZipCode));
                return Ok();
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// apply for tier 3 verification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("private/user/applyfortier3")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> GetVerifyForTier3([FromBody]Tier3Param param)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetVerifyForTier2 Call Recevied, parameters:"+param);
                }
                MemoryStream memoryStream = null;
                if (!string.IsNullOrEmpty(param.FileName))
                {
                    memoryStream = new MemoryStream();
                    var provider = new MultipartFormDataStreamProvider(ServerUploadFolder);
                    await Request.Content.ReadAsMultipartAsync(provider);
                    foreach (var file in provider.Contents)
                    {
                        file.CopyToAsync(memoryStream);
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("GetVerifyForTier2 File Recevied");
                        }
                    }
                }
                _userTierLevelApplicationService.ApplyForTier3Verification(
                    new VerifyTier3Command(HeaderParamUtility.GetApikey(Request), param.Ssn, param.Nin,
                        param.DocumentType, param.FileName, memoryStream));
                return Ok();
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetVerifyForTier2 Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

       /// <summary>
       /// get users tier statuses
       /// </summary>
       /// <returns></returns>
        [HttpGet]
        [Route("private/user/tiers")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(UserTierStatusRepresentation))]
        public IHttpActionResult GetTierStatuses()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetTierStatuses Call Recevied");
                }
                return Ok(_userTierLevelApplicationService.GetTierLevelStatuses(HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTierStatuses Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTierStatuses Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTierStatuses Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTierStatuses Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// get users tier1 details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("private/user/tier1")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(Tier1Details))]
        public IHttpActionResult GetTier1Details()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetTier1Details Call Recevied");
                }
                return Ok(_userTierLevelApplicationService.GetTier1Details(HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier1Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier1Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier1Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier1Details Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// get users tier 2 details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("private/user/tier2")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(Tier2Details))]
        public IHttpActionResult GetTier2Details()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetTier2Details Call Recevied");
                }
                return Ok(_userTierLevelApplicationService.GetTier2Details(HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// get users tier 3 details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("private/user/tier3")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(Tier3Details))]
        public IHttpActionResult GetTier3Details()
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("GetTier2Details Call Recevied");
                }
                return Ok(_userTierLevelApplicationService.GetTier3Details(HeaderParamUtility.GetApikey(Request)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("GetTier2Details Call Exception ", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// FOR ADMIN INTERFACE USE ONLY: Admin can verify a Tier from here for a user. May need a special authorization filter
        /// </summary>
        /// <param name="verifyTierLevelParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("private/user/verifytierlevel")]
        [FilterIP]
        [Authorize]
        [ResponseType(typeof(VerifyTierLevelResponse))]
        public IHttpActionResult VerifyTierLevel(VerifyTierLevelParams verifyTierLevelParams)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Verify Tier Level Call Recevied");
                }
                return Ok(_userTierLevelApplicationService.VerifyTierLevel(new VerifyTierLevelCommand(
                    verifyTierLevelParams.ApiKey, verifyTierLevelParams.TierLevel)));
            }
            catch (InvalidOperationException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Verify Tier Level Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (InvalidCredentialException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Verify Tier Level Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Verify Tier Level Call Exception ", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Verify Tier Level Call Exception ", exception);
                }
                return InternalServerError();
            }
        }
    }
}
