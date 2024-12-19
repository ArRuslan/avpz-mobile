namespace UniMobileProject.src.Views;

public partial class AdminScannerPage : ContentPage
{
	public AdminScannerPage()
	{
		InitializeComponent();
	}

	private void cameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        var barcode = e.Results.FirstOrDefault();
        if (barcode != null)
        {
            // Stop scanning and navigate back
            Dispatcher.Dispatch(async () =>
            {
                await Shell.Current.GoToAsync("..", new Dictionary<string, object>
                {
                    { "ScannedToken", barcode.Value }
                });
            });
        }
    }
}