using UniMobileProject.src.Models.ServiceModels.RoomModels;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Configuration;

namespace UniMobileProject.src.Views
{
    public partial class RoomDetailsPage : ContentPage
    {
        private readonly RoomModel _room;

        public RoomDetailsPage(RoomModel room)
        {
            InitializeComponent();
            _room = room;
            DisplayRoomDetails();
        }

        private void DisplayRoomDetails()
        {
            RoomTypeLabel.Text = _room.Type;
            RoomPriceLabel.Text = $"{_room.Price:C}";
            RoomAvailableLabel.Text = _room.Available ? "Available" : "Not Available";
        }

        private async void OnBookClicked(object sender, EventArgs e)
        {
            if (!_room.Available)
            {
                await DisplayAlert("Unavailable", "This room is not available for booking.", "OK");
                return;
            }

            var bookingPopup = new RoomBookPopup(_room);

            bookingPopup.BookingCompleted += OnBookingCompleted;

            await Navigation.PushModalAsync(bookingPopup);
        }

        private async void OnBookingCompleted(object sender, BookingData bookingData)
        {
            await DisplayAlert(
                "Booking Confirmed",
                $"Room {_room.Type} has been booked from {bookingData.CheckInDate:MM/dd/yyyy} to {bookingData.CheckOutDate:MM/dd/yyyy}.",
                "OK"
            );

            // Повернення до попередньої сторінки
            await Navigation.PopAsync();
        }
    }
}
