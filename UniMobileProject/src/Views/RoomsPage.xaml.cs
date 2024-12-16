using System;
using UniMobileProject.src.Models.ServiceModels.RoomModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.PageServices.Room;
using UniMobileProject.src.Services.Deserialization;
using Microsoft.Extensions.Configuration;

namespace UniMobileProject.src.Views
{
    public partial class RoomsPage : ContentPage
    {
        private readonly RoomService _roomService;
        private readonly int _hotelId;

        private DateTime? _checkInDate = null;
        private DateTime? _checkOutDate = null;
        decimal? _minPrice = null;
        decimal? _maxPrice = null;

        public RoomsPage(int hotelId)
        {
            InitializeComponent();

            _hotelId = hotelId;
            _roomService = new RoomService(new HttpServiceFactory(), new DeserializationFactory());
            LoadRooms();
        }

        private async Task LoadRooms(
            string? type = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            DateTime? checkIn = null,
            DateTime? checkOut = null)
        {
            // ������������ ���� � ��������� ������ "yyyy-MM-dd", ���� ��� �����������
            string? checkInString = checkIn?.ToString("yyyy-MM-dd");
            string? checkOutString = checkOut?.ToString("yyyy-MM-dd");

            var roomsResponse = await _roomService.GetRooms(
                _hotelId,
                type: type,
                priceMin: priceMin,
                priceMax: priceMax,
                checkIn: checkInString,
                checkOut: checkOutString);

            if (roomsResponse != null)
            {
                RoomsListView.ItemsSource = roomsResponse.Result;
            }
            else
            {
                await DisplayAlert("Error", "Failed to load rooms.", "OK");
            }
        }
        
        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            // ��������� ��������� �������� �� ��������� �����
            string type = TypeSearchBar.Text?.Trim() ?? string.Empty; 

            // ��������� ������� � �������� ���������, ������� ����
            await LoadRooms(type, null, null, _checkInDate, _checkOutDate);
        }

        private async void OnRoomSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is RoomModel selectedHotel)
            {
                await Navigation.PushAsync(new RoomDetailsPage(selectedHotel));

                ((CollectionView)sender).SelectedItem = null;
            }
        }

        private async void OnFilterButtonClicked(object sender, EventArgs e)
        {
            var filtersPopup = new RoomFilterPopup(_minPrice, _maxPrice, _checkInDate, _checkOutDate);

            filtersPopup.FiltersApplied += OnFiltersApplied;

            await Navigation.PushModalAsync(filtersPopup);
        }
        private async void OnSearchResetButtonPressed(object sender, EventArgs e)
        {
            TypeSearchBar.Text = "";

            _checkInDate = null;
            _checkOutDate = null;
            _minPrice = null;
            _maxPrice = null;

            await LoadRooms();
        }

        private async void OnFiltersApplied(object sender, FilterData e)
        {
            // ��������� ������� �������
            string? type = TypeSearchBar.Text?.Trim();

            _checkInDate = e.CheckInDate;
            _checkOutDate = e.CheckOutDate;
            _maxPrice = e.MaxPrice;
            _minPrice = e.MinPrice;

            // ������������ ��������� ������� ��� ������������ �����
            await LoadRooms(
                type,
                e.MinPrice,
                e.MaxPrice,
                e.CheckInDate,
                e.CheckOutDate);
        }
    }
}
