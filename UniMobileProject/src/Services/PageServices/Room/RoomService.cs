﻿using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels.RoomModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.src.Services.PageServices.Room
{
    public class RoomService
    {
        private HttpClient _httpClient;
        private ISerializer _serializer;

        public RoomService(IHttpServiceFactory httpServiceFactory, ISerializationFactory serializationFactory)
        {
            _httpClient = httpServiceFactory.Create("rooms").GetClient();
            _serializer = serializationFactory.Create(Enums.SerializerType.Room);
        }

        public async Task<PaginatedResponse<RoomModel>?> GetRooms(int hotelId, int page = 1, int pageSize = 5, string? type = null, decimal? priceMin = null, decimal? priceMax = null)
        {
            string query = $"?hotel_id={hotelId}&page={page}&page_size={pageSize}";

            if (!string.IsNullOrEmpty(type)) query += $"&type={type}";
            if (priceMin.HasValue) query += $"&price_min={priceMin.Value}";
            if (priceMax.HasValue) query += $"&price_max={priceMax.Value}";

            try
            {
                string fullUrl = new Uri(new Uri(_httpClient.BaseAddress!.ToString().TrimEnd('/')), "rooms" + query).ToString();
                Console.WriteLine($"Final URL: {fullUrl}");

                var response = await _httpClient.GetAsync(fullUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return await _serializer.Deserialize<PaginatedResponse<RoomModel>>(responseContent);
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