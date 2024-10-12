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
        public void EmailValidation_Correct_True()
        {
            string email = "someemail@gmail.com";

            var response = _service.EmailValidation(email);

            Assert.True(response.Item1);
            Assert.Null(response.Item2);
        }
    }
}
