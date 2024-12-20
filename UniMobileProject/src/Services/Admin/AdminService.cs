using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.AdminModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Database.Models;
using UniMobileProject.src.Services.Deserialization;
using UniMobileProject.src.Services.Http;

namespace UniMobileProject.src.Services.Admin
{
    public class AdminService
    {
        private TokenMaintainer _tokenMaintainer;
        private HttpClient _httpClient;
        private IDeserializer deserializer;
        public AdminService(string testdb = null)
        {
            IHttpServiceFactory httpServiceFactory = new HttpServiceFactory();
            _httpClient = httpServiceFactory.Create("admin").GetClient();
            if (testdb == null)
            {
                _tokenMaintainer = new TokenMaintainer();
            }
            else
            {
                _tokenMaintainer = new TokenMaintainer(testdb);
            }

            IDeserializationFactory factory = new DeserializationFactory();
            deserializer = factory.Create(Enums.DeserializerType.Admin);
        }

        public async Task<RequestResponse> VerifyTicket(string token)
        {
            HeaderTokenService.AddTokenToHeader(_tokenMaintainer, ref _httpClient);
            //_httpClient.DefaultRequestHeaders.Add("token", token);
            string tet = Uri.EscapeDataString(token);
            var getResponse = await _httpClient.GetAsync($"bookings/verify?token={Uri.EscapeDataString(token)}");

            RequestResponse response;
            string body = await getResponse.Content.ReadAsStringAsync();
            if (getResponse.IsSuccessStatusCode)
            {
                response = await deserializer.Deserialize<RoomSuccessModel>(body);
            }
            else
            {
                response = await deserializer.Deserialize<ErrorResponse>(body);
            }
            return response;
        }
    }
}
