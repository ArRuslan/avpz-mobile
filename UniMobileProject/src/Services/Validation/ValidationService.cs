using System.Net.Mail;
using PhoneNumbers;
using System.Globalization;

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
                "Password should contain at least one numeric character"),
                (password => !ContainsNonEnglishCharacters(password),
                    "Password should not contain non-English or unsupported characters"),
                    (password => !password.Contains(" "), "Password is not valid")
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
        public (bool, string?) ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return (false, "Email was null or empty");

            if (ContainsNonEnglishCharacters(email))
                return (false, "Email should not contain non-English characters");
            if (email.Contains(" "))
                return (false, "Email was not in the right form");

            MailAddress? address;
            bool isSuccessful = MailAddress.TryCreate(email, out address);
            return isSuccessful == true ? (isSuccessful, null) :
                (isSuccessful, "Email was not in the right form");
        }


        public (bool, string?) ValidatePassword(string password)
        {
            foreach (var (condition, message) in passwordRules)
            {
                if (!condition(password)) return (false, message);
            }
            return (true, null);
        }

        public (bool, string?) ValidatePhoneNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
                return (false, "Number is null or empty");

            if (number.Any(c => char.IsWhiteSpace(c)))
                return (false, "Phone number is not valid");

            try
            {
                var phoneNumberUtil = PhoneNumberUtil.GetInstance();
                var phoneNumber = phoneNumberUtil.Parse(number, null);

                if (!phoneNumberUtil.IsValidNumber(phoneNumber))
                    return (false, "Phone number is not valid");

                return (true, null);
            }
            catch (NumberParseException)
            {
                return (false, "Phone number is not valid");
            }
        }
        private bool ContainsNonEnglishCharacters(string input)
        {
            return input.Any(ch => ch > 127);
        }

        public (bool, string?) ValidateName(string input)
        {
            bool isCorrect = input.All(a => Char.IsAsciiLetter(a));
            return isCorrect ? 
                (isCorrect, null) : 
                (isCorrect, "Firstname and lastname should contain only latin letters");
        }
    }
}
