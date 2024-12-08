using System.Collections.ObjectModel;
using System.Windows.Input;
using UniMobileProject.src.Models.ServiceModels.HotelModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.PageServices.Hotels;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.src.Views
{
    public partial class MainPage : ContentPage
    {
        private HotelService _hotelService;
        private string _currentSearchProperty = "Name";
        public ObservableCollection<HotelModel> Hotels { get; set; }
        private CancellationTokenSource _cancellationTokenSource;
        public ICommand NavigateToRoomsCommand { get; private set; }

        public MainPage()
        {
            InitializeComponent();

            var httpServiceFactory = new HttpServiceFactory();
            var serializationFactory = new SerializationFactory();

            _hotelService = new HotelService(httpServiceFactory, serializationFactory);

            Hotels = new ObservableCollection<HotelModel>();
            BindingContext = this;

            // Инициализация команды для навигации
            NavigateToRoomsCommand = new Command<HotelModel>(async (hotel) => await NavigateToRooms(hotel));

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            SearchPropertyPicker.SelectedIndex = 0;
            SearchHotels.Placeholder = $"Search by {_currentSearchProperty}";

            await LoadHotels();
        }

        private async Task LoadHotels(string? name = null, string? address = null, string? description = null)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            try
            {
                var response = await _hotelService.GetHotels(page: 1, pageSize: 50, name: name, address: address, description: description);
                if (response != null && this.IsVisible)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        if (token.IsCancellationRequested) return;

                        BindingContext = null;
                        Hotels.Clear();
                        foreach (var hotel in response.Result)
                        {
                            Hotels.Add(hotel);
                        }
                        BindingContext = this;
                    });
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("LoadHotels operation canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching hotels: {ex.Message}");
            }
        }

        private void OnSearchPropertyChanged(object sender, EventArgs e)
        {
            if (SearchPropertyPicker.SelectedIndex != -1)
            {
                _currentSearchProperty = SearchPropertyPicker.SelectedItem.ToString();
                SearchHotels.Placeholder = $"Search by {_currentSearchProperty}";
            }
        }

        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            string searchText = SearchHotels.Text ?? string.Empty;

            // Perform search based on selected property
            switch (_currentSearchProperty)
            {
                case "Name":
                    await LoadHotels(name: searchText);
                    break;
                case "Address":
                    await LoadHotels(address: searchText);
                    break;
                case "Description":
                    await LoadHotels(description: searchText);
                    break;
                default:
                    await LoadHotels();
                    break;
            }
        }

        private async void OnSearchResetButtonPressed(object sender, EventArgs e)
        {
            SearchHotels.Text = "";

            await LoadHotels();
        }

        private async void OnHotelSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is HotelModel selectedHotel)
            {
                await Navigation.PushAsync(new HotelDetailsPage(selectedHotel));

                ((CollectionView)sender).SelectedItem = null;
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
