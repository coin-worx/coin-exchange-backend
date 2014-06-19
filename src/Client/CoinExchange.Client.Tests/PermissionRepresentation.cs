using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Client.Tests
{
    public class PermissionRepresentation
    {
        public bool Allowed { get; set; }
        public Permission Permission { get; set; }

        public PermissionRepresentation()
        {
            
        }
    }
}
