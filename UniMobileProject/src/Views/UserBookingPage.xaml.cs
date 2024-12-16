using QRCoder;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.BookingModels;
using UniMobileProject.src.Services.PageServices.Booking;

namespace UniMobileProject.src.Views;

public partial class UserBookingPage : ContentPage
{
	private SuccessfulBooking _booking = new SuccessfulBooking();
	private BookingService _bookingService = new BookingService();

	public UserBookingPage()
	{
		InitializeComponent();
	}

	public UserBookingPage(SuccessfulBooking booking)
	{
		_booking = booking;
		InitializeComponent();
		DisplayBookingDetails();
    }

	private void DisplayBookingDetails()
	{
        bookingId.Text = _booking.Id.ToString();
        bookingCheckIn.Text = _booking.CheckIn;
        bookingCheckOut.Text = _booking.CheckOut;
        bookingTotalPrice.Text = $"{_booking.TotalPrice:C}";
        bookingStatus.Text = char.ToUpper(_booking.Status.ToString()[0]) + _booking.Status.ToString().Substring(1).ToLower();
    }

    private async void CancelBookingOnClicked(object sender, EventArgs e)
    {
		RequestResponse? response = await _bookingService.CancelBooking(_booking.Id);
		if (response != null)
		{
			ErrorResponse errorResponse = (ErrorResponse)response;
            string errors = string.Join('\n', errorResponse.Errors);
			await DisplayAlert("Error", errors, "Ok");
		}
		else
		{
			await Navigation.PopAsync();
		}
    }

    private async void GetQRToScan(object sender, EventArgs e)
    {
		RequestResponse serverResponse = await _bookingService.GetTokenForQR(_booking.Id);
		if (!serverResponse.IsSuccess)
		{
			ErrorResponse errorResponse = (ErrorResponse)serverResponse;
            string errors = string.Join('\n', errorResponse.Errors);
			await DisplayAlert("Error", errors, "Ok");
			return;
		}

		BookingQrModel tokenModel = (BookingQrModel)serverResponse;

		if(DateTimeOffset.UtcNow.ToUnixTimeSeconds() < tokenModel.ExpiresIn)
		{
			await DisplayAlert("Error", "QR code is expired, try again", "Ok");
			return;
		}

		var qrCodeStream = GenerateQrCode(tokenModel.Token);
		var qrCodeImage = ImageSource.FromStream(() => qrCodeStream);

		QRBookingImage.Source = qrCodeImage;
        QRBookingImage.IsVisible = true;
    }

	private Stream GenerateQrCode(string token)
	{
		var qrGenerator = new QRCodeGenerator();
		var qrCodeData = qrGenerator.CreateQrCode(token, QRCodeGenerator.ECCLevel.Q);
		var qrCode = new PngByteQRCode(qrCodeData);
		return new MemoryStream(qrCode.GetGraphic(20));
	}
}