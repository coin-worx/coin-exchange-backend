using System;
using System.Collections.Generic;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Application.Tests
{
    public class MockLedgerRepository : ILedgerRepository
    {
        public Ledger GetLedgerById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Ledger> GetLedgerByAccountId(AccountId accountId)
        {
            throw new NotImplementedException();
        }

        public List<Ledger> GetLedgerByCurrencyName(string currency)
        {
            throw new NotImplementedException();
        }

        public Ledger GetLedgerByLedgerId(string ledgerId)
        {
            throw new NotImplementedException();
        }

        public List<Ledger> GetLedgersByTradeId(string tradeId)
        {
            throw new NotImplementedException();
        }

        public List<Ledger> GetLedgersByDepositId(string depositId)
        {
            throw new NotImplementedException();
        }

        public List<Ledger> GetLedgersByWithdrawId(string withdrawId)
        {
            throw new NotImplementedException();
        }

        public List<Ledger> GetLedgersByOrderId(string orderId)
        {
            throw new NotImplementedException();
        }

        public double GetBalanceForCurrency(string currency, AccountId accountId)
        {
            throw new NotImplementedException();
        }

        public IList<Ledger> GetAllLedgers()
        {
            throw new NotImplementedException();
        }
    }
}
