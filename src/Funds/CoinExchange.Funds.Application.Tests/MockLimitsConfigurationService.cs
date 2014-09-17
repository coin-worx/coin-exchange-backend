using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockLimitsConfigurationService : ILimitsConfigurationService
    {
        public void ConfigureCurrencyType()
        {
            
        }

        public bool EvaluateDepositLimits(string baseCurrency, string tierLevel, decimal amount, IList<Ledger> depositLedgers)
        {
            return true;
        }

        public bool EvaluateWithdrawLimits(string baseCurrency, string tierLevel, decimal amount, IList<Withdraw> withdrawals, 
            decimal availableBalance, decimal currentBalance)
        {
            return true;
        }

        public void AssignDepositLimits(string baseCurrency, string tierLevel, IList<Ledger> depositLedgers)
        {
            throw new NotImplementedException();
        }


        public void AssignDepositLimits(string baseCurrency, string tierLevel, List<Ledger> depositLedgers)
        {
            throw new NotImplementedException();
        }

        public void AssignWithdrawLimits(string baseCurrency, string tierLevel, IList<Withdraw> withdrawals, 
            decimal availableBalance, decimal currentBalance)
        {
            
        }

        public decimal ConvertCurrencyToFiat(string currency, decimal amount)
        {
            return amount*491;
        }
    }
}
