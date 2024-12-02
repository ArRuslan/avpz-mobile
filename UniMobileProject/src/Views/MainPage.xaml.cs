using System.Collections.ObjectModel;
using UniMobileProject.src.Models.ServiceModels.HotelModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.PageServices.Hotels;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.src.Views
{
    public partial class MainPage : ContentPage
    {
        private HotelService _hotelService;
        public ObservableCollection<HotelModel> Hotels { get; set; }

        public MainPage()
        {
            InitializeComponent();
            _hotelService = new HotelService(
                new HttpServiceFactory(),
                new SerializationFactory()
            );
            Hotels = new ObservableCollection<HotelModel>();
            BindingContext = this;
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
            // ѕолучаем текст из всех строк поиска
            string name = NameSearchBar.Text ?? string.Empty;
            string address = AddressSearchBar.Text ?? string.Empty;
            string description = DescriptionSearchBar.Text ?? string.Empty;

            // ≈сли все строки поиска пусты, загружаем все отели
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(address) && string.IsNullOrWhiteSpace(description))
            {
                await LoadHotels();
                return;
            }

            // ¬ыполн€ем поиск с текущими значени€ми
            await LoadHotels(name, address, description);
        }
    }
}
