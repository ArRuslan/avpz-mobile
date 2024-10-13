using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniMobileProject.src.Services.Validation;

namespace UniMobileProject.Tests.ServiceTests
{
    public class ValidationServiceTest
    {
        private ValidationService _service;
        public ValidationServiceTest()
        {
            _service = new ValidationService();
        }

        [Fact]
        public void EmailValidation_NullInput_False()
        {
            //Arrange
            string email = null;

            //Action
            var response = _service.EmailValidation(email);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Email was null or empty", response.Item2);
        }

        [Fact]
        public void EmailValidation_EmptyInput_False()
        {
            //Arrange
            string email = "";

            //Action
            var response = _service.EmailValidation(email);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Email was null or empty", response.Item2);
        }

        [Fact]
        public void EmailValidation_WrongEmailForm_False()
        {
            //Arrange
            string email = "asdvxcsda.com";

            //Action
            var response = _service.EmailValidation(email);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Email was not in the right form", response.Item2);
        }

        [Fact]
        public void EmailValidation_CyrillicCharacters_False()
        {
            // Arrange
            string email = "тест@example.com";

            // Act
            var response = _service.EmailValidation(email);

            // Assert
            Assert.False(response.Item1);
            Assert.Equal("Email should not contain non-English characters", response.Item2);
        }


        [Fact]
        public void EmailValidation_Correct_True()
        {
            //Arrange
            string email = "someemail@gmail.com";

            //Action
            var response = _service.EmailValidation(email);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }

        [Fact]
        public void PasswordValidation_NullInput_False()
        {
            //Arrange
            string password = null;

            //Action
            var response = _service.PasswordValidation(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password was null", response.Item2);
        }

        [Fact]
        public void PasswordValidation_EmptyInput_False()
        {
            //Arrange
            string password = "";

            //Action
            var response = _service.PasswordValidation(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password was null", response.Item2);
        }

        [Fact]
        public void PasswordValidation_ShortLength_False()
        {
            //Arrange
            string password = "abc";

            //Action
            var response = _service.PasswordValidation(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal($"Password should be longer than {ValidationService.MIN_PASSWORD_LENGTH} characters", response.Item2);
        }


        [Fact]
        public void PasswordValidation_NoSpecialCharacter_False()
        {
            //Arrange
            string password = "Abcde73asdf";

            //Action
            var response = _service.PasswordValidation(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password should have at least one special character", response.Item2);
        }

        [Fact]
        public void PasswordValidation_NoUpperCase_False()
        {
            //Arrange
            string password = "bcde73asdf_";

            //Action
            var response = _service.PasswordValidation(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password should contain at least one upper case character", response.Item2);
        }

        [Fact]
        public void PasswordValidation_NoLowerCase_False()
        {
            //Arrange
            string password = "BCSADFA123_";

            //Action
            var response = _service.PasswordValidation(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password should contain at least one lower case character", response.Item2);
        }


        [Fact]
        public void PasswordValidation_NoNumbers_False()
        {
            //Arrange
            string password = "BCSADFAasd_";

            //Action
            var response = _service.PasswordValidation(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password should contain at least one numeric character", response.Item2);
        }

        [Fact]
        public void PasswordValidation_Correct_True()
        {
            //Arrange
            string password = "BCSADFAasd_12";

            //Action
            var response = _service.PasswordValidation(password);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }

        [Fact]
        public void PhoneNumberValidation_NullInput_False()
        {
            //Arrange
            string number = null;

            //Action
            var response = _service.PhoneNumberValidation(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Number is null or empty", response.Item2);
        }

        [Fact]
        public void PhoneNumberValidation_EmptyInput_False()
        {
            //Arrange
            string number = "";

            //Action
            var response = _service.PhoneNumberValidation(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Number is null or empty", response.Item2);
        }

        [Fact]
        public void PhoneNumberValidation_WrongNumer_False()
        {
            //Arrange
            string number = "2312412412";

            //Action
            var response = _service.PhoneNumberValidation(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }

        [Fact]
        public void PhoneNumberValidation_WrongInternationalNumber_False()
        {
            //Arrange
            string number = "+3802312412412";

            //Action
            var response = _service.PhoneNumberValidation(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }

        [Fact]
        public void PhoneNumberValidation_CorrectUA_True()
        {
            //Arrange
            string number = "+380914819608";

            //Action
            var response = _service.PhoneNumberValidation(number);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }

        [Fact]
        public void PhoneNumberValidation_CorrectUS_True()
        {
            //Arrange
            string number = "+12154567890";

            //Action
            var response = _service.PhoneNumberValidation(number);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }

        [Fact]
        public void EmailValidation_EmailWithSpecialChars_True()
        {
            //Arrange
            string email = "user.name+tag@example.com";

            //Action
            var response = _service.EmailValidation(email);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }

        

        [Fact]
        public void PhoneNumberValidation_NumberWithSpaces_False()
        {
            //Arrange
            string number = "+38 091 481 9608";

            //Action
            var response = _service.PhoneNumberValidation(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }


        [Fact]
        public void PhoneNumberValidation_InvalidCountryCode_False()
        {
            //Arrange
            string number = "+999914819608";

            //Action
            var response = _service.PhoneNumberValidation(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }

        [Fact]
        public void PhoneNumberValidation_ShortPhoneNumber_False()
        {
            //Arrange
            string number = "+38091";

            //Action
            var response = _service.PhoneNumberValidation(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }

        [Fact]
        public void PhoneNumberValidation_LongPhoneNumber_False()
        {
            //Arrange
            string number = "+38091481960812345"; // Слишком длинный номер

            //Action
            var response = _service.PhoneNumberValidation(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }

        [Fact]
        public void PhoneNumberValidation_ValidPhoneNumberWithRegionCode_True()
        {
            //Arrange
            string number = "+380914819608";

            //Action
            var response = _service.PhoneNumberValidation(number);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }


        [Fact]
        public void EmailValidation_ChineseCharacters_False()
        {
            // Arrange
            string email = "用户@example.com"; // "用户" means "user" in Chinese

            // Act
            var response = _service.EmailValidation(email);

            // Assert
            Assert.False(response.Item1);
            Assert.Equal("Email should not contain non-English characters", response.Item2);
        }

        [Fact]
        public void EmailValidation_ArabicCharacters_False()
        {
            // Arrange
            string email = "اختبار@example.com"; // "اختبار" means "test" in Arabic

            // Act
            var response = _service.EmailValidation(email);

            // Assert
            Assert.False(response.Item1);
            Assert.Equal("Email should not contain non-English characters", response.Item2);
        }

        [Fact]
        public void EmailValidation_EmailWithJapaneseCharacters_False()
        {
            // Arrange
            string email = "テスト@example.com"; // "テスト" means "test" in Japanese

            // Act
            var response = _service.EmailValidation(email);

            // Assert
            Assert.False(response.Item1);
            Assert.Equal("Email should not contain non-English characters", response.Item2);
        }

        [Fact]
        public void EmailValidation_WhiteSpaceInEmail_False()
        {
            //Arrange
            string email = "user @example.com";

            //Action
            var response = _service.EmailValidation(email);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Email was not in the right form", response.Item2);
        }

        [Fact]
        public void PasswordValidation_WithSpaces_False()
        {
            // Arrange
            string password = "Pass word 123!";

            // Act
            var response = _service.PasswordValidation(password);

            // Assert
            Assert.False(response.Item1);
            Assert.Equal("Password is not valid", response.Item2);
        }

    }
}
