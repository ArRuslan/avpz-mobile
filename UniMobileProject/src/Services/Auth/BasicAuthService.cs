using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.src.Services.Auth
{
    public class BasicAuthService
    {
        private HttpService _httpService;
        private ISerializer _serializer;
        public BasicAuthService(IHttpServiceFactory httpServiceFactory, ISerializationFactory serializationFactory)
        {
            _httpService = httpServiceFactory.Create("auth");
            _serializer = serializationFactory.Create(Enums.SerializerType.Auth);
        }

        public async Task<bool> Login(LoginModel model)
        {
            StringContent contentToSend = _serializer.Serialize<LoginModel>(model);
            var response = await _httpService.GetClient().PostAsync("login", contentToSend) ??
                throw new ArgumentNullException("Response from the server was not received. " +
                "Internal server error happened");

            if (response.IsSuccessStatusCode)
            {
                string? token = await response.Content.ReadAsStringAsync();
                return true;
            }
            else
            {
                string? errorMessage = await response.Content.ReadAsStringAsync();
                return false;
            }
        }

        public void Register(RegisterModel model)
        {
            throw new NotImplementedException();
        }

    }
}
