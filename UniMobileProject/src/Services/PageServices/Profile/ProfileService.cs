using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Models.ServiceModels.ProfileModels;
using UniMobileProject.src.Services.Database;
using UniMobileProject.src.Services.Database.Models;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.src.Services.PageServices.Profile
{
    public class ProfileService
    {
        private HttpClient _httpClient;
        private DatabaseService _dbService;
        private ISerializer _serializer;
        public ProfileService(IHttpServiceFactory httpServiceFactory, ISerializationFactory serializationFactory, string testDb = null)
        {
            _httpClient = httpServiceFactory.Create("user").GetClient();
            _serializer = serializationFactory.Create(Enums.SerializerType.Profile);
            _dbService = testDb != null ? new DatabaseService(testDb) : new DatabaseService();
        }

        //Returns null when the token is expired
        public async Task<RequestResponse?> GetProfileModel()
        {
            Token token = await _dbService.GetToken();
            if (token.ExpiresAtTimeSpan < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                return null;
            }
            _httpClient.DefaultRequestHeaders.Add("x-token", token.TokenString);
            var clientResponse = await _httpClient.GetAsync("info");
            string clientResponseString = await clientResponse.Content.ReadAsStringAsync();
            RequestResponse serializedResponse;
            if (clientResponse.IsSuccessStatusCode)
            {
                serializedResponse = await _serializer.Deserialize<ProfileModel>(clientResponseString);
            }
            else
            {
                serializedResponse = await _serializer.Deserialize<ErrorResponse>(clientResponseString);
            }
            return serializedResponse;
        }
        public async Task<RequestResponse> EnableMfa(EnableMfaModel model)
        {
            string json = _serializer.Serialize(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("mfa/enable", httpContent);
                string responseContent = await response.Content.ReadAsStringAsync();

                RequestResponse result;
                if (response.IsSuccessStatusCode)
                {
                    result = await _serializer.Deserialize<ProfileModel>(responseContent);
                }
                else
                {
                    result = await _serializer.Deserialize<ErrorResponse>(responseContent);
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during EnableMfa: {ex.Message}");
                throw;
            }
        }

        public async Task<RequestResponse> DisableMfa(DisableMfaModel model)
        {
            string json = _serializer.Serialize<DisableMfaModel>(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/mfa/disable", httpContent) ??
                throw new ArgumentNullException("Response from the server was not received. Internal server error happened");

            return response.IsSuccessStatusCode
                ? await _serializer.Deserialize<RequestResponse>(await response.Content.ReadAsStringAsync())
                : throw new InvalidOperationException("Failed to disable MFA.");
        }
    }
}
