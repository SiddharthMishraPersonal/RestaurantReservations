using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Restaurant.Reservations.Shared.Helper;

namespace Restaurant.Reservations.ValidationRules
{
  public class NullOrEmptyValueValidationRule : ValidationRule
  {
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
      if (value == null || string.IsNullOrEmpty(value.ToString()))
      {
        return new ValidationResult(false, Constants.ValueRequiredError);
      }
      return new ValidationResult(true, null);
    }
  }
}