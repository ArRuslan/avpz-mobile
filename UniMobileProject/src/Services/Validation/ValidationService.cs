using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using PhoneNumbers;

namespace UniMobileProject.src.Services.Validation
{
    public class ValidationService
    {
        public const short MIN_PASSWORD_LENGTH = 8;
        private readonly char[] specialCharaters = new char[] {'!', '@', '#', '$', '%', '^',
        '&', '*', '_', '-', '+', '=', ':', ';', '\'', '\"', '?', '<', '>', ',', '.'};
        private List<(Func<string, bool> condition, string errorMessage)> passwordRules;
        private List<(Func<string, bool> condition, string errorMesssage)> phoneNumberRules;
        public ValidationService()
        {
            //TODO - add more rules, cause using cyryllic or special characters that are not specified
            //is not being checked
            passwordRules = new List<(Func<string, bool>, string)>()
            {
                (password => !string.IsNullOrEmpty(password), "Password was null"),
                (password => password.Length >= MIN_PASSWORD_LENGTH,
                $"Password should be longer than {MIN_PASSWORD_LENGTH} " +
                $"characters" ),
                (password => password.Any(ch => specialCharaters.Contains(ch)),
                "Password should have at least one special character"),
                (password => password.Any(ch => char.IsAsciiLetterUpper(ch)),
                "Password should contain at least one upper case character"),
                (password => password.Any(ch => char.IsAsciiLetterLower(ch)),
                "Password should contain at least one lower case character"),
                (password => password.Any(ch => char.IsNumber(ch)),
                "Password should contain at least one numeric character")
            };
            //phoneNumberRules = new List<(Func<string, bool>, string)>()
            //{
            //    (number => number[0] == '+',
            //    "Phone number should start with a '+' sign and then with your country code"),
            //    (number => number.Count(ch => ch == '+') == 1, 
            //    "Phone number should have only one '+' sign on the begining"),
            //    (number => number.Skip(1).All(ch => char.IsNumber(ch)),
            //    "Phone number should consist only from 1 '+' sign on the begining and numbers")
            //};

        }
        public (bool, string?) EmailValidation(string email)
        {
            if (string.IsNullOrEmpty(email)) return (false, "Email was null or empty");
            MailAddress? address;
            bool isSuccessful = MailAddress.TryCreate(email, out address);
            return isSuccessful == true ? (isSuccessful, null) :
                (isSuccessful, "Email was not in the right form");

        }

        public (bool, string?) PasswordValidation(string password)
        {
            foreach (var (condition, message) in passwordRules)
            {
                if (!condition(password)) return (false, message);
            }
            return (true, null);
        }

        public (bool, string?) PhoneNumberValidation(string number)
        {
            if (string.IsNullOrEmpty(number)) return (false, "Number is null or empty");
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var phoneNumber = phoneNumberUtil.Parse(number, "UA"); // UA is a default region for number check
            var isValid = phoneNumberUtil.IsValidNumber(phoneNumber);
            if (!isValid) return (false, "Phone number is not valid");
            return (true, null);
        }
    }
}
