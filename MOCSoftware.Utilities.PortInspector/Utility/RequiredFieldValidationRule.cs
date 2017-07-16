using System.Globalization;
using System.Windows.Controls;

namespace MOCSoftware.Utilities.PortInspector.Utility
{
    internal class RequiredFieldValidationRule : ValidationRule
    {
        internal string InitialValue { get; set; }
        internal string ErrorMessage { get; set; }

        private string _validationMessage;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(ValidateInternal((value ?? string.Empty).ToString()), _validationMessage);
        }

        private bool ValidateInternal(string input)
        {
            var result = !input.Equals(InitialValue ?? string.Empty);

            if (!result)
                _validationMessage = ErrorMessage;

            return result;
        }
    }
}
