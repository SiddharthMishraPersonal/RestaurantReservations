using System.Globalization;
using System.IO;
using System.Windows.Controls;
using Restaurant.Reservations.Shared.Helper;

namespace Restaurant.Reservations.ValidationRules
{
  public class FileNotFoundValidationRule : ValidationRule
  {
    public string ErrorMessage { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      if (value == null || !File.Exists(value.ToString()))
      {
        var errorMessage = string.IsNullOrEmpty(ErrorMessage)
          ? Constants.FileNotFoundError
          : string.Format("{0}\r\nFile: '{1}'", Constants.FileNotFoundError, ErrorMessage);
        return new ValidationResult(false, errorMessage);
      }
      else
      {
        return new ValidationResult(true, null);
      }
    }
  }
}