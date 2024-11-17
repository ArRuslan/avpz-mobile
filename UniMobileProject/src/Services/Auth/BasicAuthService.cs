using System.Text;
using System.Text.Json;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Serialization;
using UniMobileProject.src.Views;

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

        public async Task RequestMfaFlow(string mfaToken)
        {
            // Открытие MfaPage для ввода кода
            await Application.Current.MainPage.Navigation.PushAsync(new MfaPage(this, mfaToken));
        }

        public async Task<RequestResponse> Login(LoginModel model)
        {
            string json = _serializer.Serialize<LoginModel>(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpService.GetClient().PostAsync("login", httpContent) ??
                throw new ArgumentNullException("Response from the server was not received. Internal server error happened");

            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using (var document = JsonDocument.Parse(responseBody))
                {
                    var root = document.RootElement;

                    // Если сервер возвращает mfa_token, инициируем MFA flow
                    if (root.TryGetProperty("mfa_token", out var mfaTokenProperty))
                    {
                        string mfaToken = mfaTokenProperty.GetString()!;
                        Console.WriteLine("MFA token received. Redirecting to MFA Page...");

                        // Запускаем MfaPage
                        await RequestMfaFlow(mfaToken);
                        return new SuccessfulAuth { IsSuccess = true }; // Переход к MFA
                    }
                }

                // Обычная успешная авторизация
                RequestResponse successfulResponse = await _serializer.Deserialize<SuccessfulAuth>(responseBody);
                return successfulResponse;
            }
            else
            {
                // Обработка ошибок
                Console.WriteLine($"Error Response Body: {responseBody}");
                Console.WriteLine($"Error Status Code: {response.StatusCode}");

                RequestResponse unsuccessfulResponse = await _serializer.Deserialize<FailedAuth>(responseBody);
                return unsuccessfulResponse;
            }
        }

        public async Task<RequestResponse> LoginWithMfa(MfaLoginModel model)
        {
            string json = _serializer.Serialize<MfaLoginModel>(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpService.GetClient().PostAsync("login/mfa", httpContent) ??
                throw new ArgumentNullException("Response from the server was not received. Internal server error happened");

            if (response.IsSuccessStatusCode)
            {
                string? token = await response.Content.ReadAsStringAsync();
                RequestResponse successfulResponse = await _serializer.Deserialize<SuccessfulAuth>(token);
                return successfulResponse;
            }
            else
            {
                string? errorMessage = await response.Content.ReadAsStringAsync();
                RequestResponse unsuccessfulResponse = await _serializer.Deserialize<FailedAuth>(errorMessage);
                return unsuccessfulResponse;
            }
        }


        public async Task<RequestResponse> Register(RegisterModel model)
        {
            string json = _serializer.Serialize<RegisterModel>(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpService.GetClient().PostAsync("register", httpContent) ??
                throw new ArgumentNullException("Response from the server was not received. " +
                "Internal server error happened");

            if (response.IsSuccessStatusCode)
            {
                string? token = await response.Content.ReadAsStringAsync();
                RequestResponse successfulResponse = await _serializer.Deserialize<SuccessfulAuth>(token);
                return successfulResponse;
            }
            else
            {
                string? errors = await response.Content.ReadAsStringAsync();
                RequestResponse unsuccessfulResponse = await _serializer.Deserialize<FailedAuth>(errors);
                return unsuccessfulResponse;
            }
        }
        public async Task<bool> RequestPasswordReset(string email)
        {
            var payload = new { email = email };
            string json = _serializer.Serialize(payload);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpService.GetClient().PostAsync("reset-password/request", httpContent) ??
                throw new ArgumentNullException("Response from the server was not received. Internal server error happened");

            return response.IsSuccessStatusCode;
        }

    }
}