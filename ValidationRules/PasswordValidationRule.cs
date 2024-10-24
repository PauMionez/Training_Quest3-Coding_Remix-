using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Training_Quest3.ValidationRules
{
    internal class PasswordValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string password = value as string;
            List<string> PasswordError = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordError.Add("Password cannot be empty.");
            }
            else
            {
                if (!Regex.IsMatch(password, @"[A-Z]"))
                {
                    PasswordError.Add("1 uppercase letter");
                }

                if (!Regex.IsMatch(password, @"\d"))
                {
                    PasswordError.Add("1 digit");
                }

                if (!Regex.IsMatch(password, @"[@#$!%*?&]"))
                {
                    PasswordError.Add("1 special character");
                }

                if (password.Length < 8)
                {
                    PasswordError.Add("8 characters long");
                }

                string JoinedPasswordError = PasswordError.Count > 0 ? string.Join(", ", PasswordError) : null;

                return JoinedPasswordError == null ? new ValidationResult(true, "") : new ValidationResult(false, $"Password must contain {JoinedPasswordError}");

            }

            return ValidationResult.ValidResult;
        }

    }
}
