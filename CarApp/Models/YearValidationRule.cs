using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CarApp.Models
{
    public class YearValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int year;
            int.TryParse(value as string, out year);
            if (year > DateTime.Now.Year || year < 1900) return new ValidationResult(false, "Invalid year.");

            return ValidationResult.ValidResult;
        }
    }
}
