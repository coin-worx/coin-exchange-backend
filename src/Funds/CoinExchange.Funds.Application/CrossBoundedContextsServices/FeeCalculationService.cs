using System;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Service reposnsible for matters related to calculation and retreival of fee
    /// </summary>
    public class FeeCalculationService : IFeeCalculationService
    {
        private IFeeRepository _feeRepository;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public FeeCalculationService(IFeeRepository feeRepository)
        {
            _feeRepository = feeRepository;
        }

        #region Fee Retreival

        /// <summary>
        /// Gets the percentage fee for the given amount
        /// </summary>
        /// <returns></returns>
        public double GetFee(Currency currency, double amount)
        {
            int amountInInt = Convert.ToInt32(amount);
            double finalFee = 0;
            if (amountInInt < 1000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 1000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 1000 && amountInInt < 2000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 2000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 2000 && amountInInt < 3500)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 3500);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 3500 && amountInInt < 5000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 4000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 5000 && amountInInt < 6500)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 6500);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 6500 && amountInInt < 8000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 8000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 8000 && amountInInt < 10000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 10000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 10000 && amountInInt < 12500)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 10000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 12500 && amountInInt < 15000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 10000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 15000 && amountInInt < 17500)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 10000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 17500 && amountInInt < 20000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 10000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 20000 && amountInInt < 25000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 25000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 25000 && amountInInt < 30000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 30000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 30000 && amountInInt < 40000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 40000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 40000 && amountInInt < 50000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 50000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 50000 && amountInInt < 60000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 60000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 60000 && amountInInt < 80000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 80000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 80000 && amountInInt < 100000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 100000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 100000 && amountInInt < 125000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 125000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 125000 && amountInInt < 150000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 150000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 150000 && amountInInt < 200000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 200000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 200000 && amountInInt < 350000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 350000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 350000 && amountInInt < 500000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 500000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 500000 && amountInInt < 750000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 750000);
                finalFee = fee.PercentageFee * amount;
            }
            else if (amountInInt >= 750000 && amountInInt < 1000000)
            {
                Fee fee = _feeRepository.GetFeeByCurrencyAndAmount(currency.Name, 1000000);
                finalFee = fee.PercentageFee * amount;
            }
            return finalFee;
        }

        #endregion Fee Retreival
    }
}
