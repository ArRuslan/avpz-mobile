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
        public void ValidateEmail_NullInput_False()
        {
            //Arrange
            string email = null;

            //Action
            var response = _service.ValidateEmail(email);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Email was null or empty", response.Item2);
        }

        [Fact]
        public void ValidateEmail_EmptyInput_False()
        {
            //Arrange
            string email = "";

            //Action
            var response = _service.ValidateEmail(email);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Email was null or empty", response.Item2);
        }

        [Fact]
        public void ValidateEmail_WrongEmailForm_False()
        {
            //Arrange
            string email = "asdvxcsda.com";

            //Action
            var response = _service.ValidateEmail(email);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Email was not in the right form", response.Item2);
        }

        [Fact]
        public void ValidateEmail_CyrillicCharacters_False()
        {
            // Arrange
            string email = "тест@example.com";

            // Act
            var response = _service.ValidateEmail(email);

            // Assert
            Assert.False(response.Item1);
            Assert.Equal("Email should not contain non-English characters", response.Item2);
        }


        [Fact]
        public void ValidateEmail_Correct_True()
        {
            //Arrange
            string email = "someemail@gmail.com";

            //Action
            var response = _service.ValidateEmail(email);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }

        [Fact]
        public void ValidatePassword_NullInput_False()
        {
            //Arrange
            string password = null;

            //Action
            var response = _service.ValidatePassword(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password was null", response.Item2);
        }

        [Fact]
        public void ValidatePassword_EmptyInput_False()
        {
            //Arrange
            string password = "";

            //Action
            var response = _service.ValidatePassword(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password was null", response.Item2);
        }

        [Fact]
        public void ValidatePassword_ShortLength_False()
        {
            //Arrange
            string password = "abc";

            //Action
            var response = _service.ValidatePassword(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal($"Password should be longer than {ValidationService.MIN_PASSWORD_LENGTH} characters", response.Item2);
        }


        [Fact]
        public void ValidatePassword_NoSpecialCharacter_False()
        {
            //Arrange
            string password = "Abcde73asdf";

            //Action
            var response = _service.ValidatePassword(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password should have at least one special character", response.Item2);
        }

        [Fact]
        public void ValidatePassword_NoUpperCase_False()
        {
            //Arrange
            string password = "bcde73asdf_";

            //Action
            var response = _service.ValidatePassword(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password should contain at least one upper case character", response.Item2);
        }

        [Fact]
        public void ValidatePassword_NoLowerCase_False()
        {
            //Arrange
            string password = "BCSADFA123_";

            //Action
            var response = _service.ValidatePassword(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password should contain at least one lower case character", response.Item2);
        }


        [Fact]
        public void ValidatePassword_NoNumbers_False()
        {
            //Arrange
            string password = "BCSADFAasd_";

            //Action
            var response = _service.ValidatePassword(password);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Password should contain at least one numeric character", response.Item2);
        }

        [Fact]
        public void ValidatePassword_Correct_True()
        {
            //Arrange
            string password = "BCSADFAasd_12";

            //Action
            var response = _service.ValidatePassword(password);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }

        [Fact]
        public void ValidatePhoneNumber_NullInput_False()
        {
            //Arrange
            string number = null;

            //Action
            var response = _service.ValidatePhoneNumber(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Number is null or empty", response.Item2);
        }

        [Fact]
        public void ValidatePhoneNumber_EmptyInput_False()
        {
            //Arrange
            string number = "";

            //Action
            var response = _service.ValidatePhoneNumber(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Number is null or empty", response.Item2);
        }

        [Fact]
        public void ValidatePhoneNumber_WrongNumer_False()
        {
            //Arrange
            string number = "2312412412";

            //Action
            var response = _service.ValidatePhoneNumber(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }

        [Fact]
        public void ValidatePhoneNumber_WrongInternationalNumber_False()
        {
            //Arrange
            string number = "+3802312412412";

            //Action
            var response = _service.ValidatePhoneNumber(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }

        [Fact]
        public void ValidatePhoneNumber_CorrectUA_True()
        {
            //Arrange
            string number = "+380914819608";

            //Action
            var response = _service.ValidatePhoneNumber(number);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }

        [Fact]
        public void ValidatePhoneNumber_CorrectUS_True()
        {
            //Arrange
            string number = "+12154567890";

            //Action
            var response = _service.ValidatePhoneNumber(number);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }

        [Fact]
        public void ValidateEmail_EmailWithSpecialChars_True()
        {
            //Arrange
            string email = "user.name+tag@example.com";

            //Action
            var response = _service.ValidateEmail(email);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }

        

        [Fact]
        public void ValidatePhoneNumber_NumberWithSpaces_False()
        {
            //Arrange
            string number = "+38 091 481 9608";

            //Action
            var response = _service.ValidatePhoneNumber(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }


        [Fact]
        public void ValidatePhoneNumber_InvalidCountryCode_False()
        {
            //Arrange
            string number = "+999914819608";

            //Action
            var response = _service.ValidatePhoneNumber(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }

        [Fact]
        public void ValidatePhoneNumber_ShortPhoneNumber_False()
        {
            //Arrange
            string number = "+38091";

            //Action
            var response = _service.ValidatePhoneNumber(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }

        [Fact]
        public void ValidatePhoneNumber_LongPhoneNumber_False()
        {
            //Arrange
            string number = "+38091481960812345"; // Слишком длинный номер

            //Action
            var response = _service.ValidatePhoneNumber(number);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Phone number is not valid", response.Item2);
        }

        [Fact]
        public void ValidatePhoneNumber_ValidPhoneNumberWithRegionCode_True()
        {
            //Arrange
            string number = "+380914819608";

            //Action
            var response = _service.ValidatePhoneNumber(number);

            //Assert
            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }


        [Fact]
        public void ValidateEmail_ChineseCharacters_False()
        {
            // Arrange
            string email = "用户@example.com"; // "用户" means "user" in Chinese

            // Act
            var response = _service.ValidateEmail(email);

            // Assert
            Assert.False(response.Item1);
            Assert.Equal("Email should not contain non-English characters", response.Item2);
        }

        [Fact]
        public void ValidateEmail_ArabicCharacters_False()
        {
            // Arrange
            string email = "اختبار@example.com"; // "اختبار" means "test" in Arabic

            // Act
            var response = _service.ValidateEmail(email);

            // Assert
            Assert.False(response.Item1);
            Assert.Equal("Email should not contain non-English characters", response.Item2);
        }

        [Fact]
        public void ValidateEmail_EmailWithJapaneseCharacters_False()
        {
            // Arrange
            string email = "テスト@example.com"; // "テスト" means "test" in Japanese

            // Act
            var response = _service.ValidateEmail(email);

            // Assert
            Assert.False(response.Item1);
            Assert.Equal("Email should not contain non-English characters", response.Item2);
        }

        [Fact]
        public void ValidateEmail_WhiteSpaceInEmail_False()
        {
            //Arrange
            string email = "user @example.com";

            //Action
            var response = _service.ValidateEmail(email);

            //Assert
            Assert.False(response.Item1);
            Assert.Equal("Email was not in the right form", response.Item2);
        }

        [Fact]
        public void ValidatePassword_WithSpaces_False()
        {
            // Arrange
            string password = "Pass word 123!";

            // Act
            var response = _service.ValidatePassword(password);

            // Assert
            Assert.False(response.Item1);
            Assert.Equal("Password is not valid", response.Item2);
        }

    }
}
