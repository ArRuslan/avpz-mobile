using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels.HotelModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Deserialization;

namespace UniMobileProject.src.Services.PageServices.Hotels
{
    public class HotelService
    {
        private HttpClient _httpClient;
        private IDeserializer _deserializer;

        public HotelService(IHttpServiceFactory httpServiceFactory, IDeserializationFactory serializationFactory)
        {
            _httpClient = httpServiceFactory.Create("hotels").GetClient();
            _deserializer = serializationFactory.Create(Enums.DeserializerType.Hotel);
        }

        public async Task<PaginatedResponse<HotelModel>?> GetHotels(int page = 1, int pageSize = 5, string? name = null, string? address = null, string? description = null)
        {
            string query = $"?page={page}&page_size={pageSize}";

            if (!string.IsNullOrEmpty(name)) query += $"&name={name}";
            if (!string.IsNullOrEmpty(address)) query += $"&address={address}";
            if (!string.IsNullOrEmpty(description)) query += $"&description={description}";

            try
            {
                string fullUrl = new Uri(new Uri(_httpClient.BaseAddress!.ToString().TrimEnd('/')), "hotels" + query).ToString();

                Console.WriteLine($"Final URL: {fullUrl}");

                var response = await _httpClient.GetAsync(fullUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return await _deserializer.Deserialize<PaginatedResponse<HotelModel>>(responseContent);
                }
                else
                {
                    Console.WriteLine($"Ошибка при получении данных: {response.StatusCode}");
                    return null; 
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
                return null; 
            }
        }

    }
}
