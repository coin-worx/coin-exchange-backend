using System.Collections.Generic;
using System.Linq;
using CoinExchange.Funds.Application.LedgerServices.Representations;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Application.LedgerServices
{
    /// <summary>
    /// Service to query ledgers
    /// </summary>
    public class LedgerQueryService : ILedgerQueryService
    {
        private ILedgerRepository _ledgerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LedgerQueryService(ILedgerRepository ledgerRepository)
        {
            _ledgerRepository = ledgerRepository;
        }

        /// <summary>
        /// Gets all the ledgers
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public IList<LedgerRepresentation> GetAllLedgers(int accountId, string currency)
        {
            IList<LedgerRepresentation> ledgerRepresentations = new List<LedgerRepresentation>();
            IList<Ledger> ledgers = _ledgerRepository.GetLedgerByAccountIdAndCurrency(currency, new AccountId(accountId));
            foreach (var ledger in ledgers)
            {
                ledgerRepresentations.Add(new LedgerRepresentation(ledger.LedgerId, ledger.DateTime,
                    ledger.LedgerType.ToString(), ledger.Currency.Name, ledger.Amount, ledger.AmountInUsd, ledger.Fee, ledger.Balance,
                    ledger.TradeId, ledger.OrderId, ledger.WithdrawId, ledger.DepositId, ledger.AccountId.Value));
            }

            if (!ledgerRepresentations.Any())
            {
                ledgerRepresentations = null;
            }

            return ledgerRepresentations;
        }

        /// <summary>
        /// Get the details for the given ledger ID
        /// </summary>
        /// <param name="ledgerId"></param>
        /// <returns></returns>
        public LedgerRepresentation GetLedgerDetails(string ledgerId)
        {
            Ledger ledger = _ledgerRepository.GetLedgerByLedgerId(ledgerId);
            return new LedgerRepresentation(ledger.LedgerId, ledger.DateTime, ledger.LedgerType.ToString(), ledger.Currency.Name,
                ledger.Amount, ledger.AmountInUsd, ledger.Fee, ledger.Balance, ledger.TradeId, ledger.OrderId, ledger.WithdrawId,
                ledger.DepositId, ledger.AccountId.Value);
        }
    }
}
