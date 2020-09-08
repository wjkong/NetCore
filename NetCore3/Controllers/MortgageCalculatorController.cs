using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetCore3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MortgageCalculatorController : ControllerBase
    {
        private readonly ILogger<MortgageCalculatorController> _logger;

        public MortgageCalculatorController(ILogger<MortgageCalculatorController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] MortgageParameters mortgageParameters)
        {
            Payment[] payments = new Payment[4];

            if (mortgageParameters.Frequency > 0 && mortgageParameters.Amortization > 0 && mortgageParameters.Principal > 0)
            {
                decimal totalMonthlyCostOfBorrow = decimal.Zero;
                int[] freq = { 12, 24, 26, 52 };
                string[] freqName = { "Monthly", "Semi-monthly", "Bi-weekly", "Weekly" };

                for (var i = 0; i < freq.Length; i++)
                {
                    payments[i] = new Payment();

                    payments[i].Frequency = freq[i];
                    payments[i].Freq = freqName[i];
                    payments[i].InstallPayment = this.CalculatePayment(mortgageParameters.Principal, mortgageParameters.Rate, mortgageParameters.Amortization, freq[i]);
                    payments[i].TotalPayment = this.CalculateTotalPayment(payments[i].InstallPayment, mortgageParameters.Amortization, freq[i]);
                    payments[i].TotalCostOfBorrow = this.CalculateCostOfBorrow(mortgageParameters.Principal, payments[i].TotalPayment);
                    payments[i].MonthlyPayment = this.CalculateMonthlyPayment(payments[i].InstallPayment, freq[i]);

                    if (freq[i] == 12)
                        totalMonthlyCostOfBorrow = payments[i].TotalCostOfBorrow;

                    payments[i].AmountSave = totalMonthlyCostOfBorrow - payments[i].TotalCostOfBorrow;

                    _logger.LogInformation(payments[i].InstallPayment.ToString());
                }
            }

            
            // Testing Round 2
            return new JsonResult(payments);
        }

        private decimal CalculateMonthlyPayment(decimal installPayment, int frequency)
        {
            var monthlyPayment = decimal.Zero;


            if (frequency == 28)
               frequency = 26;
            else if (frequency == 56)
                frequency = 52;

            monthlyPayment = installPayment * frequency / 12;
            monthlyPayment = Math.Truncate(100 * monthlyPayment) / 100;

            return monthlyPayment;
        }

        private decimal CalculateCostOfBorrow(decimal principal, decimal totalPayment)
        {
            var costOfBorrow = decimal.Zero;

            costOfBorrow = totalPayment - principal;
            costOfBorrow = Math.Truncate(100 * costOfBorrow) / 100;

            return costOfBorrow;
        }

        private decimal CalculateTotalPayment(decimal installPayment, int amortization, int frequency)
        {
            var totalPayment = decimal.Zero;

            if (frequency == 28)
                frequency = 26;
            else if (frequency == 56)
                frequency = 52;

            totalPayment = installPayment * amortization * frequency;
            totalPayment = Math.Truncate(100 * totalPayment) / 100;
            return totalPayment;
        }

        private decimal CalculatePayment(decimal principal, decimal rate, int amortization, int frequency)
        {
            var paymentPerPeriod = decimal.Zero;
            var extraPaymentPerPeriod = decimal.Zero;

            if (frequency == 28 || frequency == 56)
            {
                var monthlyPaymentAmt = this.CalculatePayment(principal, rate, amortization, 12);

                frequency = frequency == 28 ? 26 : 52;

                extraPaymentPerPeriod = monthlyPaymentAmt / frequency;
            }

            rate = rate / (frequency * 100);
            amortization = amortization * frequency;

            if (rate == 0)
                paymentPerPeriod = principal / amortization;
            else
            {
                double r = Convert.ToDouble(1 + rate);
                paymentPerPeriod = principal * rate / (1 - Convert.ToDecimal(Math.Pow(r, -amortization)));
            }

            paymentPerPeriod += extraPaymentPerPeriod;
            paymentPerPeriod = Math.Truncate(100 * paymentPerPeriod) / 100;

            return paymentPerPeriod;
        }
    }
}
