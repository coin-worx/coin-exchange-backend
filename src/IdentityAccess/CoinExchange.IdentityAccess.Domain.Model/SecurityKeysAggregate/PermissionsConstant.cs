using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Permissions id constants
    /// </summary>
    public static class PermissionsConstant
    {
        // ReSharper disable InconsistentNaming
        public const string Cancel_Order = "CO";
        public const string Place_Order = "PO";
        public const string Query_Closed_Orders = "QCOT";
        public const string Query_Funds = "QF";
        public const string Query_Ledger_Entries = "QLT";
        public const string Query_Open_Orders = "QOOT";
        public const string Withdraw_Funds = "WF";
    }
}
