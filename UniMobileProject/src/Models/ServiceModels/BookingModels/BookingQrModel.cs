using System.Text.Json.Serialization;

namespace UniMobileProject.src.Models.ServiceModels.BookingModels;

public class BookingQrModel : RequestResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
    [JsonPropertyName("expires_in")]
    public long ExpiresIn { get; set; }
    public BookingQrModel()
    {
        this.IsSuccess = true;
    }
}
