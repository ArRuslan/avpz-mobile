using UniMobileProject.src.Models.ServiceModels.RoomModels;
using UniMobileProject.src.Services.PageServices.Booking;

namespace UniMobileProject.src.Views;

public partial class RoomBookPage : ContentPage
{
    private RoomModel _roomToBook;
    private BookingService _bookingService;
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
        Console.WriteLine();
        
    }
}