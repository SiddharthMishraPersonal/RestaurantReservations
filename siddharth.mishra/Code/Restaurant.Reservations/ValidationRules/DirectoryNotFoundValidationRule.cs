using System.Globalization;
using System.IO;
using System.Windows.Controls;
using Restaurant.Reservations.Shared.Helper;

namespace Restaurant.Reservations.ValidationRules
{
  public class DirectoryNotFoundValidationRule : ValidationRule
  {
    public string ErrorMessage { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      if (value == null || !Directory.Exists(value.ToString()))
      {
        var errorMessage = string.IsNullOrEmpty(ErrorMessage)
          ? Constants.DirectoryNotFoundError
          : string.Format("{0}\r\nDirectory: '{1}'", Constants.DirectoryNotFoundError, ErrorMessage);
        return new ValidationResult(false, errorMessage);
      }
      else
      {
        return new ValidationResult(true, null);
      }
    }
  }
}