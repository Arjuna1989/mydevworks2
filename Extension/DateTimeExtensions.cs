using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extension
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly DOB)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DOB.Year;
            if(DOB> today.AddYears(-age)) age--;
            return age;

        }
    }
}