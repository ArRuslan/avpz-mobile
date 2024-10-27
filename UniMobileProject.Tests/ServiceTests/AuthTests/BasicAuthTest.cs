using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.Tests.ServiceTests.AuthTests
{
    public class BasicAuthTest
    {
        private IHttpServiceFactory _httpFactory;
        private ISerializationFactory _serializationFactory;
        private BasicAuthService service;
        private LoginModel correctLoginData = new LoginModel("user@example.com", "123456789Qwe");
        private RegisterModel correctRegisterData = new RegisterModel(
            "newemail@gmail.com", "somePassword123", "John", "Doe", "+4916092545328");

        public BasicAuthTest()
        {
            _httpFactory = new HttpServiceFactory();
            _serializationFactory = new SerializationFactory();
            service = new BasicAuthService(_httpFactory, _serializationFactory);
        }

        [Fact]
        private async void Login_CorrectData_StringToken()
        {
            var response = await service.Login(correctLoginData);
            Assert.True(response.IsSuccess);
            var successful = (SuccessfulAuth)response;
            Assert.NotNull(successful.ResponseContent);
        }

        [Fact]
        private async void Register_CorrectData_StringToken()
        {
            var response = await service.Register(correctRegisterData);
            Assert.True(response.IsSuccess);
            var successful = (SuccessfulAuth)response;
            Assert.NotNull(successful.ResponseContent);
        }
    }
}
