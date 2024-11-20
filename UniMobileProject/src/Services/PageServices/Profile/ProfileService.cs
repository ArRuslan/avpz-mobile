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
            if (!await AddTokenToHeader())
            {
                throw new Exception("Can't add token in http client");
            }
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
            string json = _serializer.Serialize(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("mfa/disable", httpContent);
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
                Console.WriteLine($"An error occurred during DisableMfa: {ex.Message}");
                throw;
            }
        }
        
        public async Task<RequestResponse?> UpdateProfile(EditProfileModel model)
        {
            if (!await AddTokenToHeader())
            {
                throw new Exception("Can't add token in http client");
            }

            string json = _serializer.Serialize<EditProfileModel>(model);
            if(json == null)
            {
                throw new ArgumentNullException("Parsed mdel can't be null");
            }

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync("info", httpContent) ??
                throw new ArgumentNullException("Response from the server was not received");

            if (response.IsSuccessStatusCode)
            {
                RequestResponse successfulResponse = await _serializer.Deserialize<ProfileModel>(await response.Content.ReadAsStringAsync());
                return successfulResponse;
            }
            else
            {
                RequestResponse unsuccessfulResponse = await _serializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync());
                return unsuccessfulResponse;
            }
        }

        private async Task<bool> AddTokenToHeader()
        {
            Token token = await _dbService.GetToken();
            var currentTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (token.ExpiresAtTimeSpan < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                return false;
            }
            if (_httpClient.DefaultRequestHeaders.Contains("x-token"))
            {
                var header = _httpClient.DefaultRequestHeaders.First(a => a.Key == "x-token");
                if (header.Value.Any(a => a == token.TokenString))
                {
                    return true;
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Remove("x-token");
                    _httpClient.DefaultRequestHeaders.Add("x-token", token.TokenString);
                    return true;
                }
            }
            _httpClient.DefaultRequestHeaders.Add("x-token", token.TokenString);
            return true;
        }
    }
}
