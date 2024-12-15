using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.BookingModels;
using UniMobileProject.src.Services.PageServices.Booking;

namespace UniMobileProject.src.Views;

public partial class UserBookingPage : ContentPage
{
	private SuccessfulBooking _booking = new SuccessfulBooking();
	private BookingService _bookingService = new BookingService();
	public UserBookingPage()
	{
		InitializeComponent();
	}

	public UserBookingPage(SuccessfulBooking booking)
	{
		_booking = booking;
		InitializeComponent();
	}

    private async void CancelBookingOnClicked(object sender, EventArgs e)
    {
		RequestResponse? response = await _bookingService.CancelBooking(_booking.Id);
		if (response != null)
		{
			ErrorResponse errorResponse = (ErrorResponse)response;
            string errors = string.Join('\n', errorResponse.Errors);
			await DisplayAlert("Error", errors, "Ok");
		}
		else
		{
			await Navigation.PopAsync();
		}
    }
}