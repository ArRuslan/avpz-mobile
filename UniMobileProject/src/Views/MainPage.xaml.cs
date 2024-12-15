using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UniMobileProject.src.Models.ServiceModels.HotelModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.PageServices.Hotels;
using UniMobileProject.src.Services.Deserialization;

namespace UniMobileProject.src.Views
{
    public partial class MainPage : ContentPage
    {
        private HotelService _hotelService;
        public ObservableCollection<HotelModel> Hotels { get; set; }
        public ICommand NavigateToRoomsCommand { get; private set; }

        public MainPage()
        {
            InitializeComponent();

            _hotelService = new HotelService(
                new HttpServiceFactory(),
                new DeserializationFactory()
            );

            Hotels = new ObservableCollection<HotelModel>();
            BindingContext = this;

            // Инициализация команды для навигации
            NavigateToRoomsCommand = new Command<HotelModel>(async (hotel) => await NavigateToRooms(hotel));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadHotels();
        }

        private async Task LoadHotels(string? name = null, string? address = null, string? description = null)
        {
            try
            {
                var response = await _hotelService.GetHotels(page: 1, pageSize: 50, name: name, address: address, description: description);
                if (response != null)
                {
                    Hotels.Clear();
                    foreach (var hotel in response.Result)
                    {
                        Hotels.Add(hotel);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching hotels: {ex.Message}");
            }
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string name = NameSearchBar.Text ?? string.Empty;
            string address = AddressSearchBar.Text ?? string.Empty;
            string description = DescriptionSearchBar.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(address) && string.IsNullOrWhiteSpace(description))
            {
                await LoadHotels();
                return;
            }

            await LoadHotels(name, address, description);
        }

        private async void OnHotelTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is HotelModel selectedHotel)
            {
                await NavigateToRooms(selectedHotel);

                // Сбрасываем выбранный элемент
                ((ListView)sender).SelectedItem = null;
            }
        }

        private async Task NavigateToRooms(HotelModel hotel)
        {
            if (hotel != null)
            {
                await Navigation.PushAsync(new RoomsPage(hotel.Id));
            }
        }
    }
}
