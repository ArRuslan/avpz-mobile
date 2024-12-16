using System.Text.Json.Serialization;

namespace UniMobileProject.src.Models.ServiceModels.BookingModels;

public class BookingRequestModel
{
    [JsonPropertyName("room_id")]
    public int RoomId { get; set; }
    [JsonPropertyName("DEBUG_DISABLE_PAST_DATES_CHECK")]
    public bool DDPDC { get; set; }
    [JsonPropertyName("check_in")]
    public string CheckIn { get; set; } = string.Empty;
    [JsonPropertyName("check_out")]
    public string CheckOut { get; set; } = string.Empty;

    public BookingRequestModel(int roomId, string checkIn, string checkOut)
    {
        RoomId = roomId;
        DDPDC = false;
        CheckIn = checkIn;
        CheckOut = checkOut;
    }

}
