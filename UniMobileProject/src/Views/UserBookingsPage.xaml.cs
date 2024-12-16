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

        if (myBookings != null && myBookings.Count > 0)
        {
            NoBookingsMessage.IsVisible = false;

            // Сортування: спершу активні, потім скасовані
            var activeBookings = myBookings.Result
                .Where(b => b.Status != BookingStatus.CANCELLED)
                .ToList();

            var cancelledBookings = myBookings.Result
                .Where(b => b.Status == BookingStatus.CANCELLED)
                .ToList();

            var sortedBookings = activeBookings.Concat(cancelledBookings).ToList();

            MyBookingsListView.ItemsSource = sortedBookings;
        }
        else
        {
            NoBookingsMessage.IsVisible = true;
        }
    }

    private async void OnBookingSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
        {
            var selectedItem = (SuccessfulBooking)e.CurrentSelection.FirstOrDefault();
            if (selectedItem != null)
            {
                await Navigation.PushAsync(new UserBookingPage(selectedItem));
                LoadBookings();
            }
        }

        // Reset selection to avoid keeping the item visually selected
        var collectionView = sender as CollectionView;
        if (collectionView != null)
        {
            collectionView.SelectedItem = null;
        }
    }

}