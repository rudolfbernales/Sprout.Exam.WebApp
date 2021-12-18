using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sprout.Exam.WebApp.Controllers
{
    public class Services
    {

        public double ComputeRegularSalary(double salary, double absentDays, double taxPercentage)
        {
            double absentCost = (salary / 22) * absentDays;
            double tax = taxPercentage;
            double taxCost = salary * tax;
            double computedSalary = salary - absentCost - taxCost;
            return Math.Round(computedSalary,2);
        }

        public double ComputeContractualSalary(double rate, double workedDays)
        {
            double computedSalary = rate * workedDays;
            return Math.Round(computedSalary, 2); 
        }

    }
}
