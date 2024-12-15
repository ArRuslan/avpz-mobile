using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Deserialization;
using UniMobileProject.src.Models.ServiceModels.HotelModels;
using UniMobileProject.src.Models.ServiceModels.BookingModels;

namespace UniMobileProject.src.Services.PageServices.MyBookings; 
public class MyBookingsService
{
    private HttpClient _client;
    private IDeserializer _deserializer;
    private TokenMaintainer _tokenMaintainer;
    public MyBookingsService(string testDb = null) {
        IDeserializationFactory serializationFactory = new DeserializationFactory();
        IHttpServiceFactory httpServiceFactory = new HttpServiceFactory();
        _client = httpServiceFactory.Create("bookings").GetClient();
        _deserializer = serializationFactory.Create(Enums.DeserializerType.MyBookings);
        _tokenMaintainer = string.IsNullOrEmpty(testDb) ? new TokenMaintainer() : new TokenMaintainer(testDb);
    }

    public async Task<PaginatedResponse<SuccessfulBooking>?> GetBookings(int page = 1, int pageSize = 5, BookingStatus stauts = BookingStatus.ALL)
    {
        string query = $"?page={page}&page_size={pageSize}";

        try
        {
            string fullUrl = new Uri(new Uri(_client.BaseAddress!.ToString().TrimEnd('/')), "bookings" + query).ToString();

            bool headerSuccess = HeaderTokenService.AddTokenToHeader(_tokenMaintainer, ref _client);
            if (!headerSuccess) throw new ArgumentNullException("Auth token couldn't be set in a header for a request");

            var response = await _client.GetAsync(fullUrl);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return await _deserializer.Deserialize<PaginatedResponse<SuccessfulBooking>>(responseBody);
            }
            else
            {
                return new PaginatedResponse<SuccessfulBooking>();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request wasn't completed successfully: {ex.Message}");
            return new PaginatedResponse<SuccessfulBooking>();
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return new PaginatedResponse<SuccessfulBooking>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unpredicted error happened: {ex.Message}");
            return new PaginatedResponse<SuccessfulBooking>();
        }
    }
}
