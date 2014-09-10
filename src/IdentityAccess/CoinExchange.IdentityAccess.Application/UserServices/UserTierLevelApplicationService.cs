using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.IdentityAccess.Application.UserServices.Commands;
using CoinExchange.IdentityAccess.Application.UserServices.Representations;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Application.UserServices
{
    /// <summary>
    /// Implementation of user tier level application service
    /// </summary>
    public class UserTierLevelApplicationService : IUserTierLevelApplicationService
    {
        private IUserRepository _userRepository;
        private ISecurityKeysRepository _securityKeysRepository;
        private IIdentityAccessPersistenceRepository _persistenceRepository;
        private IDocumentPersistence _documentPersistence;

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="securityKeysRepository"></param>
        /// <param name="persistenceRepository"></param>
        public UserTierLevelApplicationService(IUserRepository userRepository, ISecurityKeysRepository securityKeysRepository, IIdentityAccessPersistenceRepository persistenceRepository,IDocumentPersistence documentPersistence)
        {
            _userRepository = userRepository;
            _securityKeysRepository = securityKeysRepository;
            _persistenceRepository = persistenceRepository;
            _documentPersistence = documentPersistence;
        }

        /// <summary>
        /// Apply for tier 1 verification
        /// </summary>
        /// <param name="command"></param>
        public void ApplyForTier1Verification(VerifyTier1Command command)
        {
            SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(command.SystemGeneratedApiKey);
            User user = _userRepository.GetUserById(securityKeysPair.UserId);
            if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0)) == Status.Verified.ToString())
            {
                if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier1, TierLevelConstant.Tier1)) == Status.NonVerified.ToString())
                {
                    //add user phone number
                    user.UpdateTier1Information(command.FullName, command.DateOfBirth, command.PhoneNumber);
                    //update user tier 1 status
                    user.UpdateTierStatus(TierLevelConstant.Tier1, Status.Preverified);
                    _persistenceRepository.SaveUpdate(user);
                }
                else
                {
                    throw new InvalidOperationException("Tier 1 already verified or applied for verification");
                }
            }
            else
            {
                throw new InvalidOperationException("Verify Tier Level 0 First");
            }
        }

        /// <summary>
        /// Apply for tier 2 verification
        /// </summary>
        /// <param name="command"></param>
        public void ApplyForTier2Verification(VerifyTier2Command command)
        {
            if (command.ValidateCommand())
            {
                SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(command.SystemGeneratedApiKey);
                User user = _userRepository.GetUserById(securityKeysPair.UserId);
                if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier1, TierLevelConstant.Tier1)) == Status.Verified.ToString())
                {
                    if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier2, TierLevelConstant.Tier2)) == Status.NonVerified.ToString())
                    {
                        //update info
                        user.UpdateTier2Information(command.City, command.State, command.AddressLine1,
                                                    command.AddressLine2,
                                                    command.ZipCode);
                        //update tier status
                        user.UpdateTierStatus(TierLevelConstant.Tier2, Status.Preverified);
                        //update user
                        _persistenceRepository.SaveUpdate(user);
                    }
                    else
                    {
                        throw new InvalidOperationException("Tier 2 already verified or applied for verification");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Verify Tier Level 1 First");
                }
            }
        }

        /// <summary>
        /// Apply for tier 3 verification
        /// </summary>
        /// <param name="command"></param>
        public void ApplyForTier3Verification(VerifyTier3Command command)
        {
            //if (command.ValidateCommand())
            {
                SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(command.SystemGeneratedApiKey);
                User user = _userRepository.GetUserById(securityKeysPair.UserId);
                if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier2, TierLevelConstant.Tier2)) == Status.Verified.ToString())
                {
                    if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier3, TierLevelConstant.Tier3)) == Status.NonVerified.ToString())
                    {
                        //update info
                        user.UpdateTier3Information(command.SocialSecurityNumber, command.Nin);
                        //update tier status
                        user.UpdateTierStatus(TierLevelConstant.Tier3, Status.Preverified);
                        UserDocument document = _documentPersistence.PersistDocument(command.FileName,
                                                                                     Constants.USER_DOCUMENT_PATH,
                                                                                     command.DocumentStream,
                                                                                     command.DocumentType, user.Id);
                        //update user
                        _persistenceRepository.SaveUpdate(user);
                        if (document != null)
                        {
                            _persistenceRepository.SaveUpdate(document);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Tier 1 already verified or applied for verification");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Verify Tier Level 2 First");
                }
            }
        }

        /// <summary>
        /// Apply for tier 4 verification
        /// </summary>
        public void ApplyForTier4Verification()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current highest Tier level for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserTierStatusRepresentation GetTierLevel(int userId)
        {
            User user = _userRepository.GetUserById(userId);
            UserTierLevelStatus[] userTierLevelStatuses = user.GetAllTiersStatus();
            UserTierLevelStatus currentTierLevelStatus = userTierLevelStatuses.First();
            foreach (var status in userTierLevelStatuses)
            {
                if (status.Status == Status.Verified)
                {
                    currentTierLevelStatus = status;
                }
            }
            return new UserTierStatusRepresentation(currentTierLevelStatus.Status.ToString(), currentTierLevelStatus.Tier);
        }

        /// <summary>
        /// Get user tier status
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public UserTierStatusRepresentation[] GetTierLevelStatuses(string apiKey)
        {
            List<UserTierStatusRepresentation> representations=new List<UserTierStatusRepresentation>();
            SecurityKeysPair keysPair = _securityKeysRepository.GetByApiKey(apiKey);
            if (keysPair != null)
            {
                User user = _userRepository.GetUserById(keysPair.UserId);
                UserTierLevelStatus[] getLevelStatuses = user.GetAllTiersStatus();
                for (int i = 0; i < getLevelStatuses.Length; i++)
                {
                    representations.Add(new UserTierStatusRepresentation(getLevelStatuses[i].Status.ToString(),getLevelStatuses[i].Tier));
                }
                return representations.ToArray();
            }
            throw new InvalidOperationException("Invalid apiKey");
        }

        /// <summary>
        /// Get Tier1 details of the user
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public Tier1Details GetTier1Details(string apiKey)
        {
            SecurityKeysPair keysPair = _securityKeysRepository.GetByApiKey(apiKey);
            if (keysPair != null)
            {
                User user = _userRepository.GetUserById(keysPair.UserId);
                
                if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier0, TierLevelConstant.Tier0)) == Status.Verified.ToString())
                {
                    if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier1, TierLevelConstant.Tier1)) ==
                    Status.NonVerified.ToString())
                    {
                        throw new InvalidOperationException("Tier 1 details are not submitted yet.");
                    }
                    return new Tier1Details(user.PhoneNumber, user.FullName, user.DateOfBirth, user.Country);
                }
                throw new InvalidOperationException("First verify Tier 0");
            }
            throw new InvalidOperationException("key doesnot exist");
        }

        /// <summary>
        /// get tier2 details of the user
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public Tier2Details GetTier2Details(string apiKey)
        {
            SecurityKeysPair keysPair = _securityKeysRepository.GetByApiKey(apiKey);
            if (keysPair != null)
            {
                User user = _userRepository.GetUserById(keysPair.UserId);
                
                if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier1, TierLevelConstant.Tier1)) == Status.Verified.ToString())
                {
                    if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier2, TierLevelConstant.Tier2)) ==
                    Status.NonVerified.ToString())
                    {
                        throw new InvalidOperationException("Tier 2 details are not submitted yet.");
                    }
                    return new Tier2Details(user.Country, user.Address1, user.Address2, "", user.State, user.City,
                        user.ZipCode.ToString());
                }
                throw new InvalidOperationException("First verify Tier 1");
            }
            throw new InvalidOperationException("key doesnot exist");
        }

        /// <summary>
        /// Get tier 3 details of the user
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public Tier3Details GetTier3Details(string apiKey)
        {
            SecurityKeysPair keysPair = _securityKeysRepository.GetByApiKey(apiKey);
            if (keysPair != null)
            {
                User user = _userRepository.GetUserById(keysPair.UserId);
                
                if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier2, TierLevelConstant.Tier2)) == Status.Verified.ToString())
                {
                    if (user.GetTierLevelStatus(new Tier(TierLevelConstant.Tier3, TierLevelConstant.Tier3)) ==
                    Status.NonVerified.ToString())
                    {
                        throw new InvalidOperationException("Tier 3 details are not submitted yet.");
                    }
                    return new Tier3Details(user.SocialSecurityNumber, user.NationalIdentificationNumber);
                }
                throw new InvalidOperationException("First verify Tier 2");
            }
            throw new InvalidOperationException("Key doesnot exist");
        }

        /// <summary>
        /// Verify the Tier level for a certain user
        /// </summary>
        /// <param name="tierLevelCommand"> </param>
        /// <returns></returns>
        public VerifyTierLevelResponse VerifyTierLevel(VerifyTierLevelCommand tierLevelCommand)
        {
             SecurityKeysPair keysPair = _securityKeysRepository.GetByApiKey(tierLevelCommand.ApiKey);
             if (keysPair != null)
             {
                 User user = _userRepository.GetUserById(keysPair.UserId);
                 if (user != null)
                 {
                     foreach (UserTierLevelStatus userTierLevelStatuse in user.GetAllTiersStatus())
                     {
                         if (userTierLevelStatuse.Tier.TierLevel == tierLevelCommand.TierLevel)
                         {
                             if (userTierLevelStatuse.Status == Status.Preverified)
                             {
                                 userTierLevelStatuse.Status = Status.Verified;
                                 _persistenceRepository.SaveUpdate(user);
                                 return new VerifyTierLevelResponse(true,
                                                                    "Tier level " + tierLevelCommand.TierLevel +
                                                                    " verified");
                             }
                             else
                             {
                                 if (userTierLevelStatuse.Status == Status.NonVerified)
                                 {
                                     throw new InvalidOperationException(string.Format("Please apply for {0} first",
                                         tierLevelCommand.TierLevel));
                                 }
                                 else
                                 {
                                     throw new InvalidOperationException(string.Format("{0} is already verified.", 
                                         tierLevelCommand.TierLevel));
                                 }
                             }
                         }
                     }
                     throw new InstanceNotFoundException(string.Format("The provided Tier Level not found. Account ID = {0}, " +
                                                                       "Tier Level = {1}",  tierLevelCommand.ApiKey, 
                                                                       tierLevelCommand.TierLevel));
                 }
                 else
                 {
                     throw new InstanceNotFoundException(string.Format("No user found for the user ID"));
                 }
             }
             else
             {
                 throw new InstanceNotFoundException(string.Format("No Security Key found for the API key: {0}", 
                     tierLevelCommand.TierLevel));
             }
        }
    }
}
