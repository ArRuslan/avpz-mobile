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
        }
    }
}
