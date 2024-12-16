using UniMobileProject.src.Models.ServiceModels.HotelModels;
using Microsoft.Maui.Controls;

namespace UniMobileProject.src.Views
{
    public partial class HotelDetailsPage : ContentPage
    {
        private readonly HotelModel _hotel;

        public HotelDetailsPage(HotelModel hotel)
        {
            InitializeComponent();
            _hotel = hotel;
            DisplayHotelDetails();
        }

        private void DisplayHotelDetails()
        {
            HotelNameLabel.Text = _hotel.Name;
            HotelAddressLabel.Text = _hotel.Address;
            HotelDescriptionLabel.Text = _hotel.Description;
        }

        private async void OnViewRoomsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RoomsPage(_hotel.Id));
        }
    }
}
