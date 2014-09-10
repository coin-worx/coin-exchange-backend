using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Stub Implementation for FundsVlaidationService
    /// </summary>
    public class StubFundsValidationService : IFundsValidationService
    {
        public bool ValidateFundsForOrder(AccountId accountId, Currency baseCurrency, Currency quoteCurrency, decimal volume, decimal price, string orderSide, string orderId)
        {
            return true;
        }

        public Withdraw ValidateFundsForWithdrawal(AccountId accountId, Currency currency, decimal amount, TransactionId transactionId, BitcoinAddress bitcoinAddress)
        {
            throw new NotImplementedException();
        }

        public bool WithdrawalExecuted(Withdraw withdraw)
        {
            return true;
        }

        public bool DepositConfirmed(Deposit deposit)
        {
            return true;
        }

        public bool TradeExecuted(string baseCurrency, string quoteCurrency, decimal tradeVolume, decimal price, DateTime executionDateTime, string tradeId, int buyAccountId, int sellAccountId, string buyOrderId, string sellOrderId)
        {
            throw new NotImplementedException();
        }

        public bool OrderCancelled(Currency baseCurrency, Currency quoteCurrency, AccountId accountId, string orderside, string orderId, decimal openQuantity, decimal price)
        {
            throw new NotImplementedException();
        }

        public AccountDepositLimits GetDepositLimitThresholds(AccountId accountId, Currency currency)
        {
            throw new NotImplementedException();
        }

        public AccountWithdrawLimits GetWithdrawThresholds(AccountId accountId, Currency currency)
        {
            throw new NotImplementedException();
        }

        public bool IsTierVerified(int accountId, bool isCrypto)
        {
            throw new NotImplementedException();
        }
    }
}
