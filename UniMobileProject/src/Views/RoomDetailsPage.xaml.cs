using UniMobileProject.src.Models.ServiceModels.RoomModels;
using Microsoft.Maui.Controls;

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
    }
}
