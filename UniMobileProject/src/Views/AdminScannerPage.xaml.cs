using ZXing.Net.Maui;

namespace UniMobileProject.src.Views;

public partial class AdminScannerPage : ContentPage
{
    public event EventHandler<QRData>? qrVerified;

    public AdminScannerPage()
	{
		InitializeComponent();
	}

	//private void cameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
 //   {
 //       var barcode = e.Results.FirstOrDefault();
 //       if (barcode != null)
 //       {
 //           // Stop scanning and navigate back
 //           Dispatcher.Dispatch(async () =>
 //           {
 //               await Shell.Current.GoToAsync("..", new Dictionary<string, object>
 //               {
 //                   { "ScannedToken", barcode.Value }
 //               });
 //           });
 //       }
 //   }

    private void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        if (cameraView.Cameras.Count > 0)
        {
            cameraView.Camera = cameraView.Cameras.FirstOrDefault();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            });
        }
    }

    private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            barcodeResult.Text = $"{args.Result[0].BarcodeFormat}: {args.Result[0].Text}";
        });

        // Створення об'єкта для передачі даних
        var qrData = new QRData
        {
            Token = args.Result[0].Text
        };

        // Виклик події для передачі фільтрів
        qrVerified?.Invoke(this, qrData);

        Navigation.PopModalAsync();
    }

    public class QRData
    {
        public string? Token { get; set; }
    }
}