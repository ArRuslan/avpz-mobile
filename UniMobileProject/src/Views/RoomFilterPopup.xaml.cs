namespace UniMobileProject.src.Views;

public partial class RoomFilterPopup : ContentPage
{
    public event EventHandler<FilterData>? FiltersApplied;
    private DateTime? _checkInDate = null;
    private DateTime? _checkOutDate = null;
    private decimal? _minPrice = null;
    private decimal? _maxPrice = null;

    public RoomFilterPopup(decimal? minPrice, decimal? maxPrice, DateTime? checkInDate, DateTime? checkOutDate)
    {
        InitializeComponent();
        _minPrice = minPrice;
        _maxPrice = maxPrice;
        _checkInDate = checkInDate;
        _checkOutDate = checkOutDate;

        MinPriceSearchBar.Text = _minPrice?.ToString() ?? string.Empty;
        MaxPriceSearchBar.Text = _maxPrice?.ToString() ?? string.Empty;

        if (_checkInDate.HasValue)
        {
            CheckInDatePicker.Date = _checkInDate.Value;
        }

        if (_checkOutDate.HasValue)
        {
            CheckOutDatePicker.Date = _checkOutDate.Value;
        }
    }

    private async void OnApplyFiltersClicked(object sender, EventArgs e)
    {
        // Очищення попередніх помилок
        PriceError.Text = string.Empty;
        CheckInError.Text = string.Empty;

        // Зчитування значень фільтрів
        decimal? minPrice = decimal.TryParse(MinPriceSearchBar.Text, out var parsedMinPrice) ? parsedMinPrice : null;
        decimal? maxPrice = decimal.TryParse(MaxPriceSearchBar.Text, out var parsedMaxPrice) ? parsedMaxPrice : null;
        DateTime? checkInDate = CheckInDatePicker.Date != default ? CheckInDatePicker.Date : null;
        DateTime? checkOutDate = CheckOutDatePicker.Date != default ? CheckOutDatePicker.Date : null;

        if (!checkInDate.HasValue && !checkOutDate.HasValue)
        {
            CheckInError.Text = "Both check-in and check-out dates must be specified.";
            return;
        }

        bool hasErrors = false;

        if (maxPrice < minPrice)
        {
            PriceError.Text = "Max price must be greater than Min price.";
            hasErrors = true;
        }

        if (checkOutDate < checkInDate)
        {
            CheckInError.Text = "Check-out date must be later than Check-in date.";
            hasErrors = true;
        }

        if (hasErrors)
        {
            return;
        }

        // Створення об'єкта для передачі даних
        var filterData = new FilterData
        {
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate
        };

        // Виклик події для передачі фільтрів
        FiltersApplied?.Invoke(this, filterData);

        await Navigation.PopModalAsync();
    }

    private void OnResetFiltersClicked(object sender, EventArgs e)
    {
        MinPriceSearchBar.Text = string.Empty;
        MaxPriceSearchBar.Text = string.Empty;
        CheckInDatePicker.Date = DateTime.Now;
        CheckOutDatePicker.Date = DateTime.Now;
    }
}

// Клас для передачі даних фільтрів
public class FilterData
{
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
}