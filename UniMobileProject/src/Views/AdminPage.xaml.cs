using Camera.MAUI;
using IronQr;
using IronSoftware.Drawing;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.AdminModels;
using UniMobileProject.src.Services.Admin;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace UniMobileProject.src.Views;

public partial class AdminPage : ContentPage
{
    private AdminService _adminService;
    private string? _bookingToken;

	public AdminPage()
	{
		InitializeComponent();
        _adminService = new AdminService();
    }

    private async void ScanQrClicked(object sender, EventArgs e)
    {
        var qrPopup = new AdminScannerPage();

        qrPopup.qrVerified += OnQrVerified;

        await Navigation.PushModalAsync(qrPopup);
    }

    private async void OnQrVerified(object? sender, AdminScannerPage.QRData e)
    {
        _bookingToken = e.Token;

        if (!string.IsNullOrEmpty(_bookingToken))
        {
            var response = await _adminService.VerifyTicket(_bookingToken);

             MainThread.BeginInvokeOnMainThread(() =>
            {
                if (response.IsSuccess && response is RoomSuccessModel roomDetails)
                {
                    // Update UI with room details
                    RoomDetailsStack.IsVisible = true;
                    ErrorFrame.IsVisible = false;

                    RoomIdLabel.Text = roomDetails.Id.ToString();
                    RoomTypeLabel.Text = roomDetails.Type;
                    RoomPriceLabel.Text = $"{roomDetails.Price:C}";
                    RoomAvailableLabel.Text = roomDetails.Available ? "Available" : "Not Available";

                    DisplayAlert("Success", "Verification successful!", "OK");
                }
                else if (response is ErrorResponse errorResponse)
                {
                    // Update UI with error messages
                    RoomDetailsStack.IsVisible = false;
                    ErrorFrame.IsVisible = true;

                    ErrorLabel.Text = $"Error: {string.Join(", ", errorResponse.Errors)}";
                }
            });
        }
        else
        {
            await DisplayAlert("Error", "Invalid token scanned.", "OK");
        }
    }
}