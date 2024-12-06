using System;
using UniMobileProject.src.Models.ServiceModels.RoomModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.PageServices.Room;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.src.Views
{
    public partial class RoomsPage : ContentPage
    {
        private readonly RoomService _roomService;
        private readonly int _hotelId;

        private DateTime? _checkInDate = null;
        private DateTime? _checkOutDate = null;

        public RoomsPage(int hotelId)
        {
            InitializeComponent();
            _hotelId = hotelId;
            _roomService = new RoomService(new HttpServiceFactory(), new SerializationFactory());
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

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            // ��������� ��������� �������� �� ��������� �����
            string type = TypeSearchBar.Text?.Trim();
            decimal? priceMin = null;
            decimal? priceMax = null;

            // ������ ����������� ����, ���� �������
            if (decimal.TryParse(MinPriceSearchBar.Text, out decimal minPrice))
            {
                priceMin = minPrice;
            }

            // ������ ������������ ����, ���� �������
            if (decimal.TryParse(MaxPriceSearchBar.Text, out decimal maxPrice))
            {
                priceMax = maxPrice;
            }

            Console.WriteLine($"Search Parameters - Type: {type}, Price Min: {priceMin}, Price Max: {priceMax}");

            // ��������� ������� � �������� ���������, ������� ����
            await LoadRooms(type, priceMin, priceMax, _checkInDate, _checkOutDate);
        }

        private async void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            // ��������� ���� ������ ��� ������ � ����������� �� ��������� DatePicker
            if (sender == CheckInDatePicker)
            {
                _checkInDate = e.NewDate;
            }
            else if (sender == CheckOutDatePicker)
            {
                _checkOutDate = e.NewDate;
            }

            // ����� ����������� ������ ���� ������� ��� ����
            if (_checkInDate.HasValue && _checkOutDate.HasValue)
            {
                // ���������, ��� ���� ������ ����� ���� ������
                if (_checkOutDate <= _checkInDate)
                {
                    await DisplayAlert("Error", "Check-out date must be after check-in date.", "OK");
                    return;
                }

                // �������� ������� �������� �� ������ ��������
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

                Console.WriteLine($"Search Parameters - Type: {type}, Price Min: {priceMin}, Price Max: {priceMax}, Check-in: {_checkInDate}, Check-out: {_checkOutDate}");

                // ��������� �������
                await LoadRooms(type, priceMin, priceMax, _checkInDate, _checkOutDate);
            }
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
