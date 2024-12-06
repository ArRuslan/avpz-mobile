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
            // Конвертируем даты в строковый формат "yyyy-MM-dd", если они установлены
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
            // Извлекаем текстовые значения из поисковых строк
            string type = TypeSearchBar.Text?.Trim();
            decimal? priceMin = null;
            decimal? priceMax = null;

            // Парсим минимальную цену, если введена
            if (decimal.TryParse(MinPriceSearchBar.Text, out decimal minPrice))
            {
                priceMin = minPrice;
            }

            // Парсим максимальную цену, если введена
            if (decimal.TryParse(MaxPriceSearchBar.Text, out decimal maxPrice))
            {
                priceMax = maxPrice;
            }

            Console.WriteLine($"Search Parameters - Type: {type}, Price Min: {priceMin}, Price Max: {priceMax}");

            // Загружаем комнаты с текущими фильтрами, включая даты
            await LoadRooms(type, priceMin, priceMax, _checkInDate, _checkOutDate);
        }

        private async void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            // Обновляем дату заезда или выезда в зависимости от активного DatePicker
            if (sender == CheckInDatePicker)
            {
                _checkInDate = e.NewDate;
            }
            else if (sender == CheckOutDatePicker)
            {
                _checkOutDate = e.NewDate;
            }

            // Поиск выполняется только если выбраны обе даты
            if (_checkInDate.HasValue && _checkOutDate.HasValue)
            {
                // Проверяем, что дата выезда позже даты заезда
                if (_checkOutDate <= _checkInDate)
                {
                    await DisplayAlert("Error", "Check-out date must be after check-in date.", "OK");
                    return;
                }

                // Получаем текущие значения из других фильтров
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

                // Загружаем комнаты
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
