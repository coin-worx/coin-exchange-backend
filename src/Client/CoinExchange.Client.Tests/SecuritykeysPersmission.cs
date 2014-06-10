using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CoinExchange.Client.Tests
{
    
    public class SecuritykeysPersmission
    {
        [JsonProperty(PropertyName = "KeyDescritpion")]
        public string KeyDescritpion { get; set; }
        [JsonProperty(PropertyName = "EnableStartDate")]
        public bool EnableStartDate { get; set; }
        [JsonProperty(PropertyName = "EnableEndDate")]
        public bool EnableEndDate { get; set; }
        [JsonProperty(PropertyName = "EnableExpirationDate")]
        public bool EnableExpirationDate { get; set; }
        [JsonProperty(PropertyName = "EndDateTime")]
        public string EndDateTime { get; set; }
        [JsonProperty(PropertyName = "StartDateTime")]
        public string StartDateTime { get; set; }
        [JsonProperty(PropertyName = "ExpirationDateTime")]
        public string ExpirationDateTime { get; set; }
        [JsonProperty(PropertyName = "SecurityKeyPermissions")]
        public PermissionRepresentation[] SecurityKeyPermissions { get; set; }

        public SecuritykeysPersmission(string expirationDateTime, string startDateTime, string endDateTime, bool enableExpirationDate, bool enableEndDate, bool enableStartDate, string keyDescritpion,PermissionRepresentation[] securityKeyPermissions)
        {
            SecurityKeyPermissions = securityKeyPermissions;
            ExpirationDateTime = expirationDateTime;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            EnableExpirationDate = enableExpirationDate;
            EnableEndDate = enableEndDate;
            EnableStartDate = enableStartDate;
            KeyDescritpion = keyDescritpion;
       }
    }
}
