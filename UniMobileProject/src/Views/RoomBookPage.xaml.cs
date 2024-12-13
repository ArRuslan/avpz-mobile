using Microsoft.Extensions.Configuration;
using UniMobileProject.src.Models.ServiceModels.BookingModels;
using UniMobileProject.src.Models.ServiceModels.RoomModels;
using UniMobileProject.src.Services.PageServices.Booking;
using PayPalCheckoutSdk;
using System.Web;

namespace UniMobileProject.src.Views;

public partial class RoomBookPage : ContentPage
{
    private RoomModel _roomToBook;
    private BookingService _bookingService;
    private IConfiguration _configuration;
    public RoomBookPage(RoomModel room)
    {
        _roomToBook = room;
		InitializeComponent();
        _bookingService = new BookingService();
    }

    private async void OnBookClicked(object sender, EventArgs e)
    {
        DateTime checkIn = CheckIn.Date;
        DateTime checkOut = CheckOut.Date;
        if (checkIn >= checkOut)
        {
            await DisplayAlert("Error","Check out date should be bigger than check in date", "Ok");
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
            }
            catch (Exception ex)
            {

            }
        }
        Console.WriteLine();
        
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
}