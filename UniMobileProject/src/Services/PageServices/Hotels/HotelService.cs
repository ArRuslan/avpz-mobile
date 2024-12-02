using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels.HotelModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.src.Services.PageServices.Hotels
{
    public class HotelService
    {
        private HttpClient _httpClient;
        private ISerializer _serializer;

        public HotelService(IHttpServiceFactory httpServiceFactory, ISerializationFactory serializationFactory)
        {
            _httpClient = httpServiceFactory.Create("hotels").GetClient();
            _serializer = serializationFactory.Create(Enums.SerializerType.Hotel);
        }

        public async Task<PaginatedResponse<HotelModel>?> GetHotels(int page = 1, int pageSize = 10, string? name = null, string? address = null, string? description = null)
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
                    return await _serializer.Deserialize<PaginatedResponse<HotelModel>>(responseContent);
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
