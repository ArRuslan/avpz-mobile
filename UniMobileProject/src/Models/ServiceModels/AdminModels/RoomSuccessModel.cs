using System.Text.Json.Serialization;

namespace UniMobileProject.src.Models.ServiceModels.AdminModels;

public class RoomSuccessModel : RequestResponse
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

    public RoomSuccessModel()
    {
        this.IsSuccess = true;
    }
}
