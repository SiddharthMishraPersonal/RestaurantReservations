using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Restaurant.Reservations.ValidationRules
{
  public class RegexValidationRule : ValidationRule
  {
    private string _pattern;
    private Regex _regex;
    private string _errorMessage;


    public string Pattern
    {
      get { return _pattern; }
      set
      {
        _pattern = value;
        _regex = new Regex(_pattern, RegexOptions.IgnoreCase);
      }
    }

    public string ErrorMessage
    {
      get { return _errorMessage; }
      set { _errorMessage = value; }
    }

    public RegexValidationRule()
    {
    }

    public override ValidationResult Validate(object value, CultureInfo ultureInfo)
    {
      if (value == null || !_regex.Match(value.ToString()).Success)
      {
        return new ValidationResult(false, ErrorMessage);
      }
      else
      {
        return new ValidationResult(true, null);
      }
    }
  }
}