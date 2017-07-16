using System.Globalization;
using System.Windows.Controls;

namespace MOCSoftware.Utilities.PortInspector.Utility
{
    internal class RangeValidationRule : ValidationRule
    {
        internal string ErrorMessage { get; set; }
        internal int MinValue { get; set; }
        internal int MaxValue { get; set; }

        private string _validationMessage;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(ValidateInternal((value ?? string.Empty).ToString().Trim()), _validationMessage);
        }

        private bool ValidateInternal(string input)
        {
            int value;
            var isValid = int.TryParse(input, out value);

            if (isValid)
                isValid = value >= MinValue && value <= MaxValue;

            if (!isValid)
                _validationMessage = ErrorMessage;

            return isValid;
        }
    }
}
