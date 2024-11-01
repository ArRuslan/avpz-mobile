using System.Text;
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

        public async Task<AuthResponse> Login(LoginModel model)
        {
            string json = _serializer.Serialize<LoginModel>(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpService.GetClient().PostAsync("login", httpContent) ??
                throw new ArgumentNullException("Response from the server was not received. " +
                "Internal server error happened");

            if (response.IsSuccessStatusCode)
            {
                string? token = await response.Content.ReadAsStringAsync();
                AuthResponse successfulResponse = await _serializer.Deserialize<SuccessfulAuth>(token);
                return successfulResponse;
            }
            else
            {
                string? errorMessage = await response.Content.ReadAsStringAsync();
                AuthResponse unsuccessfulResponse = await _serializer.Deserialize<FailedAuth>(errorMessage);
                return unsuccessfulResponse;
            }
        }

        public async Task<AuthResponse> Register(RegisterModel model)
        {
            string json = _serializer.Serialize<RegisterModel>(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpService.GetClient().PostAsync("register", httpContent) ??
                throw new ArgumentNullException("Response from the server was not received. " +
                "Internal server error happened");

            if (response.IsSuccessStatusCode)
            {
                string? token = await response.Content.ReadAsStringAsync();
                AuthResponse successfulResponse = await _serializer.Deserialize<SuccessfulAuth>(token);
                return successfulResponse;
            }
            else
            {
                string? errors = await response.Content.ReadAsStringAsync();
                AuthResponse unsuccessfulResponse = await _serializer.Deserialize<FailedAuth>(errors);
                return unsuccessfulResponse;
            }
        }

    }
}
