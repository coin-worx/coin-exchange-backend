using System;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Service to evaluate the Maximum Deposit
    /// </summary>
    public class DepositLimitEvaluationService : IDepositLimitEvaluationService
    {
        private double _dailyLimit = 0;
        private double _dailyLimitUsed = 0;
        private double _monthlyLimit = 0;
        private double _monthlyLimitUsed = 0;
        private double _maximumDeposit = 0;
        private double _maximumDepositUsd = 0;

        /// <summary>
        /// Evaluates if the current deposit transaction is within the maximum deposit limit and is allowed to proceed
        /// </summary>
        /// <param name="currentDepositAmount"> </param>
        /// <param name="depositLedgers"></param>
        /// <param name="depositLimit"></param>
        /// <param name="bestBidPrice"></param>
        /// <param name="bestAskPrice"></param>
        /// <returns></returns>
        public bool EvaluateDepositLimit(double currentDepositAmount, IList<Ledger> depositLedgers, DepositLimit depositLimit, double bestBidPrice,
            double bestAskPrice)
        {
            // Get the midpoint of the dailylimit divided by the best bid and best ask
            //double originalBboMidpoint = ConvertUsdToCurrency(bestBidPrice, bestAskPrice, depositLimit.DailyLimit);
            // Set Daily and Monthly Limit
            SetLimits(depositLimit);
            // Set the amount used in the Daily and Monthly limit
            SetUsedLimitsUsd(depositLedgers, bestBidPrice, bestAskPrice);
            // Evaluate the Maximum Deposit, set it, and return response whether it went successfully or not
            if (EvaluateMaximumDepositUsd(bestBidPrice, bestAskPrice))
            {
                return currentDepositAmount <= _maximumDepositUsd;
            }
            return false;
        }

        /// <summary>
        /// Evaluates the value of Maximum Deposit
        /// </summary>
        /// <returns></returns>
        private bool EvaluateMaximumDepositUsd(double bestBid, double bestAsk)
        {
            // If either the daily limit or monthly limit has been reached, no deposit can be made
            if (!(_monthlyLimit - _monthlyLimitUsed).Equals(0) || !(_dailyLimit - _dailyLimitUsed).Equals(0))
            {
                // If both the used limits have not been used
                if (_monthlyLimitUsed.Equals(0) && _dailyLimitUsed.Equals(0))
                {
                    return SetMaximumDeposit(_dailyLimit, bestBid, bestAsk);
                }
                // If only the DailyLimitUsed is 0, we need to evaluate other conditions
                else if (_dailyLimitUsed.Equals(0))
                {
                    // E.g., if DailyLimit = 0/1000 & MonthlyLimit = 3000/5000
                    if (_monthlyLimit - _monthlyLimitUsed > _dailyLimit)
                    {
                        return SetMaximumDeposit(_dailyLimit, bestBid, bestAsk);
                    }
                    // E.g., if DailyLimit = 0/1000, & MonthlyLimit = 4500/5000
                    else if (_monthlyLimit - _monthlyLimitUsed < _dailyLimit)
                    {
                        return SetMaximumDeposit(_monthlyLimit - _monthlyLimitUsed, bestBid, bestAsk);
                    }
                }
                // E.g., if DailyLimit = 500/1000 & MonthlyLimit = 0/5000
                else if (_monthlyLimitUsed.Equals(0))
                {
                    return SetMaximumDeposit(_dailyLimit - _dailyLimitUsed, bestBid, bestAsk);
                }
                // E.g., if DailyLimit = 500/1000 & MonthlyLimit = 4000/5000
                else if ((_monthlyLimit - _monthlyLimitUsed) > (_dailyLimit - _dailyLimitUsed))
                {
                    return SetMaximumDeposit(_dailyLimit - _dailyLimitUsed, bestBid, bestAsk);
                }
                // E.g., if DailyLimit = 400/1000 & MonthlyLimit = 4500/5000
                else if ((_monthlyLimit - _monthlyLimitUsed) < (_dailyLimit - _dailyLimitUsed))
                {
                    return SetMaximumDeposit(_monthlyLimit - _monthlyLimitUsed, bestBid, bestAsk);
                }
            }
            return false;
        }

        /// <summary>
        /// Evaluates the value of Maximum Deposit
        /// </summary>
        /// <returns></returns>
        private bool EvaluateMaximumDeposit(double originalBboMidpoint, double bestBid, double bestAsk)
        {
            // If either the daily limit or monthly limit has been reached, no deposit can be made
            if (!(_monthlyLimit - _monthlyLimitUsed).Equals(0) || !(_dailyLimit - _dailyLimitUsed).Equals(0))
            {
                // If both the used limits have not been used
                if (_monthlyLimitUsed.Equals(0) && _dailyLimitUsed.Equals(0))
                {
                    SetMaximumDeposit(originalBboMidpoint, bestBid, bestAsk);
                }
                    // If only the DailyLimitUsed is 0, we need to evaluate other conditions
                else if (_dailyLimitUsed.Equals(0))
                {
                    // E.g., if DailyLimit = 0/1000 & MonthlyLimit = 3000/5000
                    if (_monthlyLimit - _monthlyLimitUsed > _dailyLimit)
                    {
                        return SetMaximumDeposit(originalBboMidpoint, bestBid, bestAsk);
                    }
                    // E.g., if DailyLimit = 0/1000, & MonthlyLimit = 4500/5000
                    else if (_monthlyLimit - _monthlyLimitUsed < _dailyLimit)
                    {
                        return SetMaximumDeposit(bestBid, bestAsk, _monthlyLimit - _monthlyLimitUsed);
                    }
                }
                // E.g., if DailyLimit = 500/1000 & MonthlyLimit = 0/5000
                else if (_monthlyLimitUsed.Equals(0))
                {
                    return SetMaximumDeposit(bestBid, bestAsk, _dailyLimit - _dailyLimitUsed);
                }
                // E.g., if DailyLimit = 500/1000 & MonthlyLimit = 4000/5000
                else if ((_monthlyLimit - _monthlyLimitUsed) > ConvertUsdToCurrency(bestBid, bestAsk, _dailyLimit - _dailyLimitUsed))
                {
                    return SetMaximumDeposit(bestBid, bestAsk, _monthlyLimit - _monthlyLimitUsed);
                }
                // E.g., if DailyLimit = 400/1000 & MonthlyLimit = 4500/5000
                else if ((_monthlyLimit - _monthlyLimitUsed) < (_dailyLimit - _dailyLimitUsed))
                {
                    return SetMaximumDeposit(bestBid, bestAsk, _dailyLimit - _dailyLimitUsed);
                }
            }
            return false;
        }

        /// <summary>
        /// Set the value for the Maximum Deposit in currency amount and US Dollars
        /// </summary>
        private bool SetMaximumDeposit(double maximumDeposit, double bestBid, double bestAsk)
        {
            _maximumDeposit = ConvertUsdToCurrency(bestBid, bestAsk, maximumDeposit);
            _maximumDepositUsd = maximumDeposit;
            return true;
        }

        /// <summary>
        /// Converts US Dollars to currency amount
        /// </summary>
        private double ConvertUsdToCurrency(double bestBid, double bestAsk, double usdAmount)
        {
            double sum = (usdAmount/bestBid) + (usdAmount/bestAsk);
            double midPoint = sum/2;
            return midPoint;
        }

        /// <summary>
        /// Converts currency to US Dollars
        /// </summary>
        private double ConvertCurrencyToUsd(double bestBid, double bestAsk, double currencyAmount)
        {
            double sum = (currencyAmount * bestBid) + (currencyAmount * bestAsk);
            return sum / 2;
        }

        /// <summary>
        /// Sets teh limits of daily and monthly deposit limits
        /// </summary>
        private void SetLimits(DepositLimit depositLimit)
        {
            _dailyLimit = depositLimit.DailyLimit;
            _monthlyLimit = depositLimit.MonthlyLimit;
        }

        /// <summary>
        /// Sets the amount that has been used for daily and monthly deposit limits
        /// </summary>
        /// <returns></returns>
        private void SetUsedLimits(IList<Ledger> depositLedgers, double bestBid, double bestAsk)
        {
            double tempDailyLimitUsed = 0;
            double tempMonthlyLimitUsed = 0;
            foreach (var depositLedger in depositLedgers)
            {
                if (depositLedger.DateTime >= DateTime.Now.AddHours(-24))
                {
                    tempDailyLimitUsed += depositLedger.Amount;
                    tempMonthlyLimitUsed += depositLedger.Amount;
                }
                if (depositLedger.DateTime >= DateTime.Now.AddDays(-30) && depositLedger.DateTime < DateTime.Now.AddHours(-24))
                {
                    tempMonthlyLimitUsed += depositLedger.Amount;
                }
            }
            if (tempDailyLimitUsed > 0)
            {
                // Deposit is going to be saved in the ledgers in the unit of the currency itself, we need to convert
                //it to dollars by using the following formula
                _dailyLimitUsed = (((tempDailyLimitUsed * bestBid) + (tempDailyLimitUsed * bestAsk)) / 2);
            }
            if (tempMonthlyLimitUsed > 0)
            {
                // Deposit is going to be saved in the ledgers in the unit of the currency itself, we need to convert
                //it to dollars by using the following formula
                _monthlyLimitUsed = ((tempMonthlyLimitUsed * bestBid) + (tempMonthlyLimitUsed * bestAsk))/ 2;
            }
        }

        /// <summary>
        /// Sets the amount that has been used for daily and monthly deposit limits
        /// </summary>
        /// <returns></returns>
        private void SetUsedLimitsUsd(IList<Ledger> depositLedgers, double bestBid, double bestAsk)
        {
            double tempDailyLimitUsed = 0;
            double tempMonthlyLimitUsed = 0;
            foreach (var depositLedger in depositLedgers)
            {
                if (depositLedger.DateTime >= DateTime.Now.AddHours(-24))
                {
                    tempDailyLimitUsed += depositLedger.AmountInUsd;
                    tempMonthlyLimitUsed += depositLedger.AmountInUsd;
                }
                if (depositLedger.DateTime >= DateTime.Now.AddDays(-30) && depositLedger.DateTime < DateTime.Now.AddHours(-24))
                {
                    tempMonthlyLimitUsed += depositLedger.AmountInUsd;
                }
            }

            _dailyLimitUsed = tempDailyLimitUsed;
            _monthlyLimitUsed = tempMonthlyLimitUsed;
            /*if (tempDailyLimitUsed > 0)
            {
                // Deposit is going to be saved in the ledgers in the unit of the currency itself, we need to convert
                //it to dollars by using the following formula
                _dailyLimitUsed = (((tempDailyLimitUsed * bestBid) + (tempDailyLimitUsed * bestAsk)) / 2);
            }
            if (tempMonthlyLimitUsed > 0)
            {
                // Deposit is going to be saved in the ledgers in the unit of the currency itself, we need to convert
                //it to dollars by using the following formula
                _monthlyLimitUsed = ((tempMonthlyLimitUsed * bestBid) + (tempMonthlyLimitUsed * bestAsk)) / 2;
            }*/
        }

        #region Properties

        /// <summary>
        /// Daily limit
        /// </summary>
        public double DailyLimit
        {
            get { return _dailyLimit; }
            private set { value = _dailyLimit; }
        }

        /// <summary>
        /// Daily limit that has been used in the last 24 hours
        /// </summary>
        public double DailyLimitUsed
        {
            get { return _dailyLimitUsed; }
            private set { value = _dailyLimitUsed; }
        }

        /// <summary>
        /// Monthly limit
        /// </summary>
        public double MonthlyLimit
        {
            get { return _monthlyLimit; }
            private set { value = _monthlyLimit; }
        }

        /// <summary>
        /// Monthly Limit that has been used in the last 30 days
        /// </summary>
        public double MonthlyLimitUsed
        {
            get { return _monthlyLimitUsed; }
            private set { value = _monthlyLimitUsed; }
        }

        /// <summary>
        /// Maximum Deposit allowed to the user at this moment
        /// </summary>
        public double MaximumDeposit
        {
            get { return _maximumDeposit; }
            private set { value = _maximumDeposit; }
        }

        #endregion Properties
    }
}
