using System.Text.Json.Serialization;

namespace UniMobileProject.src.Models.ServiceModels.BookingModels;

public class SuccessfulBooking : RequestResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    [JsonPropertyName("room_id")]
    public int RoomId { get; set; }
    [JsonPropertyName("check_in")]
    public string CheckIn { get; set; } = string.Empty;
    [JsonPropertyName("check_out")]
    public string CheckOut { get; set; } = string.Empty;
    [JsonPropertyName("total_price")]
    public decimal TotalPrice { get; set; }
    [JsonPropertyName("status")]
    public BookingStatus Status { get; set; }
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }
    [JsonPropertyName("payment_id")]
    public string PaymentId { get; set; } = string.Empty;
    public SuccessfulBooking()
    {
        this.IsSuccess = true;
    }
}
