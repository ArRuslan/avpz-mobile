using IronQr;
using IronSoftware.Drawing;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace UniMobileProject.src.Views;

public partial class AdminPage : ContentPage
{
	public AdminPage()
	{
		InitializeComponent();
	}

    private async void ScanQrClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdminScannerPage));
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (Shell.Current.CurrentState.Parameters.TryGetValue("ScannedToken", out var scannedToken))
        {
            if (scannedToken != null) ;
        }
    }

    //private async Task<string> CaptureAndScanQrCode()
    //{
    //    try
    //    {
    //        var photo = await MediaPicker.CapturePhotoAsync();
    //        if (photo == null) return string.Empty;

    //        using var stream = await photo.OpenReadAsync();
    //        using var memoryStream = new MemoryStream();
    //        await stream.CopyToAsync(memoryStream);

    //        var imageData = memoryStream.ToArray();

    //        // Optional: Save for debugging
    //        // File.WriteAllBytes("debug_image.jpg", imageData);

    //        QrReader reader = new QrReader();

    //        try
    //        {
    //            var inputBmp = AnyBitmap.FromBytes(imageData);
    //            Qr ImageInput imageInput = new QrImageInput(inputBmp);
    //            var barcodeResult = reader.Read(imageInput);

    //            if (barcodeResult?.Count() > 0)
    //            {
    //                return barcodeResult.First().Value;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Error processing QR code: {ex.Message}");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Error", $"Error scanning QR Code: {ex.Message}", "Ok");
    //    }

    //    return string.Empty;
    //}
}