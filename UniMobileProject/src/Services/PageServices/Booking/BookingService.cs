using System.Text;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.BookingModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Database.Models;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Deserialization;
using UniMobileProject.src.Services.Serialization;
using System.Net.Http.Json;

namespace UniMobileProject.src.Services.PageServices.Booking
{
    public class BookingService
    {
        private HttpClient _httpClient;
        private IDeserializer _deserializer;
        private TokenMaintainer _tokenMaintainer;

        public BookingService(string testDb = null)
        {
            IDeserializationFactory serializationFactory = new DeserializationFactory();
            IHttpServiceFactory httpFactory = new HttpServiceFactory();
            _httpClient= httpFactory.Create("bookings").GetClient();
            _deserializer = serializationFactory.Create(Enums.DeserializerType.Booking);
            if (string.IsNullOrEmpty(testDb)) _tokenMaintainer = new TokenMaintainer();
            else _tokenMaintainer = new TokenMaintainer(testDb);
        }
        public async Task<RequestResponse> BookRoom(int roomId, DateTime checkIn, DateTime checkOut)
        {
            string checkInStr = checkIn.ToString("yyyy-MM-dd");
            string checkOutStr = checkOut.ToString("yyyy-MM-dd");

            BookingRequestModel bookingRequest = new BookingRequestModel(roomId, checkInStr, checkOutStr);
            var json = Serializer.Serialize<BookingRequestModel>(bookingRequest);
            if (!HeaderTokenService.AddTokenToHeader(_tokenMaintainer, ref _httpClient))
            {
                return new ErrorResponse() { Errors = new List<string> {"Error appeared during room booking (couldn't get token)"} };
            }

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            string fullUrl = new Uri(new Uri(_httpClient.BaseAddress!.ToString().TrimEnd('/')), "bookings").ToString();
            var response = await _httpClient.PostAsync(fullUrl, httpContent) // default path is the endpoint
                ?? throw new ArgumentNullException("Response from the server was not received");

            string responseBody = await response.Content.ReadAsStringAsync();

            RequestResponse responseObject;
            if (response.IsSuccessStatusCode)
            {
                responseObject = await _deserializer.Deserialize<SuccessfulBooking>(responseBody);
            }
            else
            {
                responseObject = await _deserializer.Deserialize<ErrorResponse>(responseBody);
            }
            return responseObject;
            
        }

        public async Task<RequestResponse?> CancelBooking(int bookingId)
        {
            var httpContent = new StringContent(JsonContent.Create(new {bookind_id = bookingId}).ToString(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{bookingId}/cancel", httpContent) ?? throw new ArgumentNullException("Cancel request was not successful");

            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            return await _deserializer.Deserialize<ErrorResponse>(responseBody);

        }
        
        public async Task<RequestResponse> GetTokenForQR(int bookingId)
        {
            var response = await _httpClient.GetAsync($"{bookingId}/verification-token");

            RequestResponse responseObj;
            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                responseObj = await _deserializer.Deserialize<BookingQrModel>(responseBody);
            }
            else
            {
                responseObj = await _deserializer.Deserialize<ErrorResponse>(responseBody);
            }
            return responseObj;
        }
    }
}
