using System.Text;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.BookingModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Database.Models;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.src.Services.PageServices.Booking
{
    public class BookingService
    {
        private HttpService _httpService;
        private ISerializer _serializer;
        private TokenMaintainer _tokenMaintainer;

        public BookingService(string testDb = null)
        {
            ISerializationFactory serializationFactory = new SerializationFactory();
            IHttpServiceFactory httpFactory = new HttpServiceFactory();
            _httpService = httpFactory.Create("bookings");
            _serializer = serializationFactory.Create(Enums.SerializerType.Booking);
            if (string.IsNullOrEmpty(testDb)) _tokenMaintainer = new TokenMaintainer();
            else _tokenMaintainer = new TokenMaintainer(testDb);
        }
        public async Task<RequestResponse> BookRoom(int roomId, DateTime checkIn, DateTime checkOut)
        {
            string checkInStr = checkIn.ToString("yyyy-MM-dd");
            string checkOutStr = checkOut.ToString("yyyy-MM-dd");

            BookingRequestModel bookingRequest = new BookingRequestModel(roomId, checkInStr, checkOutStr);
            var json = _serializer.Serialize<BookingRequestModel>(bookingRequest);
            if (!await AddTokenToHeader())
            {
                return new ErrorResponse() { Errors = new List<string> {"Error appeared during room booking (couldn't get token)"} };
            }

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpService.GetClient().PostAsync("", httpContent) // default path is the endpoint
                ?? throw new ArgumentNullException("Response from the server was not received");

            string responseBody = await response.Content.ReadAsStringAsync();

            RequestResponse responseObject;
            if (response.IsSuccessStatusCode)
            {
                responseObject = await _serializer.Deserialize<SuccessfulBooking>(responseBody);
            }
            else
            {
                responseObject = await _serializer.Deserialize<ErrorResponse>(responseBody);
            }
            return responseObject;
            
        }

        private async Task<bool> AddTokenToHeader()
        {
            Token? token = await _tokenMaintainer.GetToken();
            if (token == null) return false;
            var currentTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (token.ExpiresAtTimeSpan < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                return false;
            }
            if (_httpService.GetClient().DefaultRequestHeaders.Contains("x-token"))
            {
                var header = _httpService.GetClient().DefaultRequestHeaders.First(a => a.Key == "x-token");
                if (header.Value.Any(a => a == token.TokenString))
                {
                    return true;
                }
                else
                {
                    _httpService.GetClient().DefaultRequestHeaders.Remove("x-token");
                    _httpService.GetClient().DefaultRequestHeaders.Add("x-token", token.TokenString);
                    return true;
                }
            }
            _httpService.GetClient().DefaultRequestHeaders.Add("x-token", token.TokenString);
            return true;
        }
    }
}
