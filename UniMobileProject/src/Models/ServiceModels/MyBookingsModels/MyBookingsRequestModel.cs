using System.Text.Json.Serialization;
using UniMobileProject.src.Models.ServiceModels.BookingModels;

namespace UniMobileProject.src.Models.ServiceModels.MyBookingsModels;
public class MyBookingsRequestModel {
    [JsonPropertyName("count")]
    public int Count { get; set; }
    [JsonPropertyName("result")]
    public List<SuccessfulBooking> Bookings { get; set; } = new List<SuccessfulBooking>();
}
