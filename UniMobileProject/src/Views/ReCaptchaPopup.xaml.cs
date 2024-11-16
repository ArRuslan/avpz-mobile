using UniMobileProject.src.Services.ReCaptcha;

namespace UniMobileProject.src.Views;

public partial class ReCaptchaPopup : ContentPage
{
    public TaskCompletionSource<string> CaptchaTokenCompletionSource { get; private set; } = new TaskCompletionSource<string>();

    public ReCaptchaPopup()
    {
        InitializeComponent();

        CaptchaWebView.Source = new HtmlWebViewSource
        {
            BaseUrl = ReCaptchaService.baseUrl,
            Html = ReCaptchaService.captchaHtml
        };

        CaptchaWebView.Navigated += OnCaptchaNavigated;
    }

    private async void OnCaptchaNavigated(object sender, WebNavigatedEventArgs e)
    {
        string captchaToken = await ReCaptchaService.HandleCaptchaNavigation(e);

        if (!string.IsNullOrEmpty(captchaToken))
        {
            // Повертаємо токен і закриваємо вікно
            CaptchaTokenCompletionSource.SetResult(captchaToken);
            await Navigation.PopModalAsync();
        }
    }
}