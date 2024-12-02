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

            try
            {
                var response = await _hotelService.GetHotels(page: 1, pageSize: 50);
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
    }
}
