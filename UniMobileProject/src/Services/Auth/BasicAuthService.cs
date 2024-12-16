using System.Text;
using System.Text.Json;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Database;
using UniMobileProject.src.Services.Database.Models;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Deserialization;
using UniMobileProject.src.Views;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.src.Services.Auth
{
    public class BasicAuthService
    {
        private HttpService _httpService;
        private IDeserializer _serializer;
        private readonly DatabaseService _dbService;
        public BasicAuthService(IHttpServiceFactory httpServiceFactory, IDeserializationFactory serializationFactory, string testDb = null)
        {
            _httpService = httpServiceFactory.Create("auth");
            _serializer = serializationFactory.Create(Enums.DeserializerType.Auth);
            if (string.IsNullOrEmpty(testDb))
            {
                _dbService = new DatabaseService();
            }
            else
            {
                _dbService = new DatabaseService(testDb);
            }
        }

        public async Task RequestMfaFlow(string mfaToken)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new MfaPage(this, mfaToken));
        }

        public async Task<RequestResponse> Login(LoginModel model)
        {
            string json = Serializer.Serialize<LoginModel>(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            // Отправляем запрос на сервер для логина
            var response = await _httpService.GetClient().PostAsync("login", httpContent) ??
                throw new ArgumentNullException("Response from the server was not received. Internal server error happened");

            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Если запрос успешный, значит, мы успешно авторизовались
                return await HandleSuccessfulLogin(responseBody, model.Email);
            }
            else
            {
                using (var document = JsonDocument.Parse(responseBody))
                {
                    var root = document.RootElement;

                    if (root.TryGetProperty("mfa_token", out var mfaTokenProperty))
                    {
                        string mfaToken = mfaTokenProperty.GetString()!;
                        Console.WriteLine("MFA token received. Redirecting to MFA Page...");
                        return new MfaRequiredAuth { MfaToken = mfaToken, IsSuccess = true };
                    }
                }

                Console.WriteLine("Login failed: Invalid credentials.");
                RequestResponse unsuccessfulResponse = await _serializer.Deserialize<FailedAuth>(responseBody);
                return unsuccessfulResponse;
            }
        }


        private async Task<RequestResponse> HandleSuccessfulLogin(string responseBody, string email)
        {
            using (var document = JsonDocument.Parse(responseBody))
            {
                var root = document.RootElement;

                if (root.TryGetProperty("token", out var tokenProperty) &&
                    root.TryGetProperty("expires_at", out var expiresAtProperty))
                {
                    string tokenString = tokenProperty.GetString()!;
                    long expiresAt = expiresAtProperty.GetInt64();

                    // Сохраняем токен в базе данных
                    var token = new Token
                    {
                        TokenString = tokenString,
                        ExpiresAtTimeSpan = expiresAt
                    };
                    Console.WriteLine($"Logged in successfully with email: {email}");
                    await _dbService.AddToken(token);
                    Console.WriteLine("Token saved to database.");
                    return new SuccessfulAuth { IsSuccess = true };  // Возвращаем успешный результат
                }
                else
                {
                    // Обработка случая, когда нет токена
                    return new FailedAuth { IsSuccess = false };
                }
            }
        }

        public async Task<RequestResponse> LoginWithMfa(MfaLoginModel model)
        {
            string json = Serializer.Serialize<MfaLoginModel>(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpService.GetClient().PostAsync("login/mfa", httpContent);

            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                // Случай успешного входа
                return await _serializer.Deserialize<SuccessfulAuth>(responseBody);
            }
            else
            {
                // Ошибка MFA
                return await _serializer.Deserialize<FailedAuth>(responseBody);
            }
        }

        public async Task<RequestResponse> Register(RegisterModel model)
        {
            string json = Serializer.Serialize<RegisterModel>(model);
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
            string json = Serializer.Serialize(payload);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpService.GetClient().PostAsync("reset-password/request", httpContent) ??
                throw new ArgumentNullException("Response from the server was not received. Internal server error happened");

            return response.IsSuccessStatusCode;
        }

    }
}