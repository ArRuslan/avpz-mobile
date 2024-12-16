using Microsoft.Extensions.Configuration;
using UniMobileProject.src.Models.ServiceModels.BookingModels;
using UniMobileProject.src.Models.ServiceModels.RoomModels;
using UniMobileProject.src.Services.PageServices.Booking;
using System.Web;

namespace UniMobileProject.src.Views;

public partial class RoomBookPopup : ContentPage
{
    private RoomModel _roomToBook;
    private BookingService _bookingService;
    private IConfiguration _configuration;

    public event EventHandler<BookingData>? BookingCompleted;

    public RoomBookPopup(RoomModel room)
	{
        _roomToBook = room;
        InitializeComponent();
        _bookingService = new BookingService();
    }

    private async void OnBookClicked(object sender, EventArgs e)
    {
        Error.Text = string.Empty;

        DateTime checkIn = CheckIn.Date;
        DateTime checkOut = CheckOut.Date;
        if (checkIn >= checkOut)
        {
            Error.Text = "Check out date should be bigger than check in date";
            return;
        }

        var response = await _bookingService.BookRoom(_roomToBook.Id, checkIn, checkOut);
        if (response.IsSuccess)
        {

            SuccessfulBooking booking = (SuccessfulBooking)response;
            if (booking == null || booking.PaymentId == null) throw new ArgumentNullException("Payment Id was null");
            try
            {
                Uri uri = BuildCheckoutLink(booking.PaymentId);
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

                // Створення об'єкта для передачі даних бронювання
                var bookingData = new BookingData
                {
                    RoomId = _roomToBook.Id,
                    CheckInDate = checkIn,
                    CheckOutDate = checkOut,
                    PaymentId = booking.PaymentId
                };

                // Виклик події для передачі результатів бронювання
                BookingCompleted?.Invoke(this, bookingData);

                // Закриття попапу після успішної оплати
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during payment process: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Booking failed.");
        }

    }

    private Uri BuildCheckoutLink(string orderId)
    {
        var uriBuilder = new UriBuilder("https://www.sandbox.paypal.com");

        uriBuilder.Path = "checkoutnow";
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["token"] = orderId;
        query["redirect_url"] = "https://example.com";
        query["native_xo"] = "1";
        query["fundingSource"] = "paypal";
        uriBuilder.Query = query.ToString();

        return uriBuilder.Uri;
    }

    private void OnCancelBookingClicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}

// Клас для передачі даних бронювання
public class BookingData
{
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string PaymentId { get; set; }
}