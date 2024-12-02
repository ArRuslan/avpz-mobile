using UniMobileProject.src.Models.ServiceModels.RoomModels;
using UniMobileProject.src.Services.PageServices.Room;
using Microsoft.Maui.Controls;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.src.Views
{
    public partial class RoomsPage : ContentPage
    {
        private readonly RoomService _roomService;
        private readonly int _hotelId;

        public RoomsPage(int hotelId)
        {
            InitializeComponent();
            _hotelId = hotelId;
            _roomService = new RoomService(new HttpServiceFactory(), new SerializationFactory());
            LoadRooms();
        }

        
        private async Task LoadRooms(string? type = null, decimal? priceMin = null, decimal? priceMax = null)
        {
            var roomsResponse = await _roomService.GetRooms(_hotelId, type: type, priceMin: priceMin, priceMax: priceMax);

            if (roomsResponse != null)
            {
                RoomsListView.ItemsSource = roomsResponse.Result;
            }
            else
            {
                await DisplayAlert("Error", "Failed to load rooms.", "OK");
            }
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string type = TypeSearchBar.Text?.Trim();
            decimal? priceMin = null;
            decimal? priceMax = null;

            if (decimal.TryParse(MinPriceSearchBar.Text, out decimal minPrice))
            {
                priceMin = minPrice;
            }

            if (decimal.TryParse(MaxPriceSearchBar.Text, out decimal maxPrice))
            {
                priceMax = maxPrice;
            }

            Console.WriteLine($"Search Parameters - Type: {type}, Price Min: {priceMin}, Price Max: {priceMax}");

            await LoadRooms(type, priceMin, priceMax);
        }


        private async void OnRoomTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var selectedRoom = (RoomModel)e.Item;
                await Navigation.PushAsync(new RoomDetailsPage(selectedRoom));
            }
        }
    }
}
