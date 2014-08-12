using System;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Service reposnsible for matters related to calculation and retreival of fee
    /// </summary>
    public class FeeCalculationService : IFeeCalculationService
    {
        private IFeeRepository _feeRepository;
        private ILedgerRepository _ledgerRepository;
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public FeeCalculationService(IFeeRepository feeRepository, ILedgerRepository ledgerRepository)
        {
            _feeRepository = feeRepository;
            _ledgerRepository = ledgerRepository;
        }

        #region Fee Retreival

        /// <summary>
        /// Gets the percentage fee for the given amount
        /// </summary>
        /// <returns></returns>
        public decimal GetFee(Currency baseCurrency, Currency quoteCurrency, AccountId accountId, decimal volume, decimal price)
        {
            // NOTE: If the quote currency is not USD, then we need to get the rate for that currency.
            // E.g., if the currency pair given is XBT/XRP, then we will get the best bid for XRP/USD and convert the rate
            // as X * XRP = (X * Best Bid(XRP/USD) + X * Best Ask(XRP/USD)) / 2, as we want to convert the XRP we have to 
            // the amount of USD that someone on top of the order book is willing to pay, but currently it is not needed,
            // as we are yet dealing with only XBT/USD.
            decimal tradesVolume = GetLast30DaysTradeVolume(quoteCurrency.Name, accountId);

            decimal percentageFee = GetFeeInstance(baseCurrency, quoteCurrency, Math.Abs(tradesVolume));

            return Math.Abs((percentageFee / 100) * (volume * price));
        }

        /// <summary>
        /// Get the Fee Instance from the database
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private decimal GetFeeInstance(Currency baseCurrency, Currency quoteCurrency, decimal amount)
        {
            string currencyPair = baseCurrency.Name + quoteCurrency.Name;
            int amountInInt = Convert.ToInt32(amount);
            decimal finalFee = 0;
            if (amountInInt < 1000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 1000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 1000 && amountInInt < 2000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 2000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 2000 && amountInInt < 3500)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 3500);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 3500 && amountInInt < 5000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 4000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 5000 && amountInInt < 6500)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 6500);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 6500 && amountInInt < 8000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 8000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 8000 && amountInInt < 10000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 10000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 10000 && amountInInt < 12500)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 10000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 12500 && amountInInt < 15000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 10000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 15000 && amountInInt < 17500)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 10000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 17500 && amountInInt < 20000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 10000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 20000 && amountInInt < 25000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 25000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 25000 && amountInInt < 30000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 30000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 30000 && amountInInt < 40000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 40000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 40000 && amountInInt < 50000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 50000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 50000 && amountInInt < 60000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 60000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 60000 && amountInInt < 80000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 80000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 80000 && amountInInt < 100000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 100000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 100000 && amountInInt < 125000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 125000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 125000 && amountInInt < 150000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 150000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 150000 && amountInInt < 200000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 200000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 200000 && amountInInt < 350000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 350000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 350000 && amountInInt < 500000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 500000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 500000 && amountInInt < 750000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 750000);
                return fee.PercentageFee;
            }
            else if (amountInInt >= 750000 && amountInInt < 1000000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, 1000000);
                return fee.PercentageFee;
            }
            return finalFee;
        }


        #endregion Fee Retreival

        #region Private Methods

        /// <summary>
        /// Gets the volume traded by a trader for a particular currency in the last 30 days in US Dollars
        /// </summary>
        /// <returns></returns>
        private decimal GetLast30DaysTradeVolume(string quoteCurrency, AccountId accountId)
        {
            decimal tradesVolume = 0;
            IList<Ledger> allLedgers = _ledgerRepository.GetLedgerByAccountIdAndCurrency(quoteCurrency, accountId);
            if (allLedgers.Any())
            {
                foreach (var ledger in allLedgers)
                {
                    if (ledger.LedgerType == LedgerType.Trade &&
                        ledger.DateTime >= DateTime.Now.AddDays(-30) &&
                        !ledger.IsBaseCurrencyInTrade)
                    {
                        tradesVolume += ledger.Amount;
                    }
                }
            }
            return tradesVolume;
        }

        #endregion Private Methods
    }
}
