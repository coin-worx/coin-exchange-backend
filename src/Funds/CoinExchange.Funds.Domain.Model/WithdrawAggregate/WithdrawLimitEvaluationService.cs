using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Service to determine the maximum Withdrawal amount
    /// </summary>
    public class WithdrawLimitEvaluationService : IWithdrawLimitEvaluationService
    {
        private decimal _dailyLimit = 0;
        private decimal _dailyLimitUsed = 0;
        private decimal _monthlyLimit = 0;
        private decimal _monthlyLimitUsed = 0;
        private decimal _maximumWithdraw = 0;
        private decimal _withheld = 0;

        /// <summary>
        /// Evaluate if the user is eligible to withdraw the desired amount or not
        /// </summary>
        /// <param name="withdrawAmount"></param>
        /// <param name="withdrawLedgers"></param>
        /// <param name="withdrawLimit"></param>
        /// <param name="availableBalance"></param>
        /// <param name="currentBalance"></param>
        /// <returns></returns>
        public bool EvaluateMaximumWithdrawLimit(decimal withdrawAmount, IList<Withdraw> withdrawLedgers,
            WithdrawLimit withdrawLimit, decimal availableBalance, 
            decimal currentBalance)
        {
            if (withdrawLimit.DailyLimit != 0 && withdrawLimit.MonthlyLimit != 0)
            {
                // Set Daily and Monthly Limit
                SetLimits(withdrawLimit);
                // Set the amount used in the Daily and Monthly limit
                SetUsedLimits(withdrawLedgers);
                // Evaluate the Maximum Withdraw, set it, and return response whether it went successfully or not
                if (EvaluateMaximumWithdrawUsd())
                {
                    // If we do not have sufficient balance, then the maximum withdrawal amount is the balance that we have 
                    // at our disposal
                    if (availableBalance < _maximumWithdraw)
                    {
                        _maximumWithdraw = Math.Round(availableBalance, 5);
                    }
                    _withheld = Math.Round(currentBalance - availableBalance, 5);

                    // If the current withdraw amount is less than the maximum withdraw
                    return withdrawAmount <= _maximumWithdraw;
                }
            }

            _maximumWithdraw = 0;
            _dailyLimit = 0;
            _dailyLimitUsed = 0;
            _monthlyLimit = 0;
            _monthlyLimitUsed = 0;

            return false;
        }

        /// <summary>
        /// Assign the limits to withdrawal
        /// </summary>
        /// <param name="withdrawLedgers"></param>
        /// <param name="withdrawLimit"></param>
        /// <param name="availableBalance"></param>
        /// <param name="currentBalance"></param>
        /// <returns></returns>
        public bool AssignWithdrawLimits(IList<Withdraw> withdrawLedgers, WithdrawLimit withdrawLimit, decimal availableBalance,
                                         decimal currentBalance)
        {
            if (withdrawLimit.DailyLimit != 0 && withdrawLimit.MonthlyLimit != 0)
            {
                // Set Daily and Monthly Limit
                SetLimits(withdrawLimit);
                // Set the amount used in the Daily and Monthly limit
                SetUsedLimits(withdrawLedgers);
                // Evaluate the Maximum Withdraw, set it, and return response whether it went successfully or not
                if (EvaluateMaximumWithdrawUsd())
                {
                    // If we do not have sufficient balance, then the maximum withdrawal amount is the balance that we have 
                    // at our disposal
                    if (availableBalance < _maximumWithdraw)
                    {
                        _maximumWithdraw = Math.Round(availableBalance, 5);
                    }
                    _withheld = Math.Round(currentBalance - availableBalance, 5);
                    return true;
                }
            }
            _maximumWithdraw = 0;
            _dailyLimit = 0;
            _dailyLimitUsed = 0;
            _monthlyLimit = 0;
            _monthlyLimitUsed = 0;

            _withheld = Math.Round(currentBalance - availableBalance, 5);

            return false;
        }

        /// <summary>
        /// Evaluates the value of Maximum Withdraw
        /// </summary>
        /// <returns></returns>
        private bool EvaluateMaximumWithdrawUsd()
        {
            // If either the daily limit or monthly limit has been reached, no wihtdraw can be made
            if (!(_monthlyLimit - _monthlyLimitUsed).Equals(0) || !(_dailyLimit - _dailyLimitUsed).Equals(0))
            {
                // If both the used limits have not been used
                if (_monthlyLimitUsed.Equals(0) && _dailyLimitUsed.Equals(0))
                {
                    return SetMaximumWithdraw(_dailyLimit);
                }
                // If only the DailyLimitUsed is 0, we need to evaluate other conditions
                else if (_dailyLimitUsed.Equals(0))
                {
                    // E.g., if DailyLimit = 0/1000 & MonthlyLimit = 3000/5000
                    if (_monthlyLimit - _monthlyLimitUsed > _dailyLimit)
                    {
                        return SetMaximumWithdraw(_dailyLimit);
                    }
                    // E.g., if DailyLimit = 0/1000, & MonthlyLimit = 4500/5000
                    else if (_monthlyLimit - _monthlyLimitUsed < _dailyLimit)
                    {
                        return SetMaximumWithdraw(_monthlyLimit - _monthlyLimitUsed);
                    }
                }
                // E.g., if DailyLimit = 500/1000 & MonthlyLimit = 4000/5000
                else if ((_monthlyLimit - _monthlyLimitUsed) > (_dailyLimit - _dailyLimitUsed))
                {
                    return SetMaximumWithdraw(_dailyLimit - _dailyLimitUsed);
                }
                // E.g., if DailyLimit = 400/1000 & MonthlyLimit = 4500/5000
                else if ((_monthlyLimit - _monthlyLimitUsed) < (_dailyLimit - _dailyLimitUsed))
                {
                    return SetMaximumWithdraw(_monthlyLimit - _monthlyLimitUsed);
                }
                else if ((_monthlyLimit - _monthlyLimitUsed).Equals(_dailyLimit - _dailyLimitUsed))
                {
                    return SetMaximumWithdraw(_monthlyLimit - _monthlyLimitUsed);
                }
            }
            return false;
        }

        /// <summary>
        /// Set the value for the Maximum withdraw in currency amount
        /// </summary>
        private bool SetMaximumWithdraw(decimal maximumWithdraw)
        {
            _maximumWithdraw = maximumWithdraw;
            return true;
        }

        /// <summary>
        /// Sets teh limits of daily and monthly Withdraw limits
        /// </summary>
        private void SetLimits(WithdrawLimit withdrawLimit)
        {
            _dailyLimit = withdrawLimit.DailyLimit;
            _monthlyLimit = withdrawLimit.MonthlyLimit;
        }

        /// <summary>
        /// Sets the amount that has been used for daily and monthly Withdraw limits
        /// </summary>
        /// <returns></returns>
        private void SetUsedLimits(IList<Withdraw> withdraws)
        {
            decimal tempDailyLimitUsed = 0;
            decimal tempMonthlyLimitUsed = 0;
            if (withdraws != null)
            {
                foreach (var withdraw in withdraws)
                {
                    if (withdraw.DateTime >= DateTime.Now.AddHours(-24))
                    {
                        tempDailyLimitUsed += withdraw.Amount;
                        tempMonthlyLimitUsed += withdraw.Amount;
                    }
                    if (withdraw.DateTime >= DateTime.Now.AddDays(-30) &&
                        withdraw.DateTime < DateTime.Now.AddHours(-24))
                    {
                        tempMonthlyLimitUsed += withdraw.Amount;
                    }
                }
            }

            _dailyLimitUsed = tempDailyLimitUsed;
            _monthlyLimitUsed = tempMonthlyLimitUsed;
        }


        /// <summary>
        /// Daily Limit
        /// </summary>
        public decimal DailyLimit { get { return _dailyLimit; } private set { _dailyLimit = value; } }

        /// <summary>
        /// Daily Limit Used
        /// </summary>
        public decimal DailyLimitUsed { get { return _dailyLimitUsed; } private set { _dailyLimitUsed = value; } }

        /// <summary>
        /// Monthly Limit
        /// </summary>
        public decimal MonthlyLimit { get { return _monthlyLimit; } private set { _monthlyLimit = value; } }

        /// <summary>
        /// Monthly limit used
        /// </summary>
        public decimal MonthlyLimitUsed { get { return _monthlyLimitUsed; } private set { _monthlyLimitUsed = value; } }

        /// <summary>
        /// Withheld Amount
        /// </summary>
        public decimal WithheldAmount { get { return _withheld; } private set { _withheld = value; } }

        /// <summary>
        /// Maximum withdraw amount
        /// </summary>
        public decimal MaximumWithdraw { get { return _maximumWithdraw; } private set { _maximumWithdraw = value; } }
    }
}
