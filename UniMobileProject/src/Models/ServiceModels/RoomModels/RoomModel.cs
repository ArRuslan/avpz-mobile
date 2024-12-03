using System.Text.Json.Serialization;

namespace UniMobileProject.src.Models.ServiceModels.RoomModels
{
    public class RoomModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("hotel_id")]
        public int HotelId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("available")]
        public bool Available { get; set; }
    }

    public class PaginatedResponse<T>
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("result")]
        public List<T> Result { get; set; } = new List<T>();
    }
}
