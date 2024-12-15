using UniMobileProject.src.Models.ServiceModels.BookingModels;
using UniMobileProject.src.Models.ServiceModels.HotelModels;
using UniMobileProject.src.Services.PageServices.MyBookings;

namespace UniMobileProject.src.Views;

public partial class UserBookingsPage : ContentPage
{
	private PaginatedResponse<SuccessfulBooking> bookings = new PaginatedResponse<SuccessfulBooking>();
	private MyBookingsService _myBookingsService = new MyBookingsService();
	public UserBookingsPage()
	{
		InitializeComponent();
		LoadBookings();
	}

	private async void LoadBookings()
	{
		var myBookings = await _myBookingsService.GetBookings();
		if (myBookings.Count > 0)
		{
			NoBookingsMessage.IsVisible = false;
			MyBookingsListView.ItemsSource = myBookings.Result;
		}
		else
		{
			NoBookingsMessage.IsVisible = true;
		}
	}

    private async void MyBookingsListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
		if (e.Item != null)
		{
			var selectedItem = (SuccessfulBooking)e.Item;
			await Navigation.PushAsync(new UserBookingPage(selectedItem));
			LoadBookings();
		}
    }
}