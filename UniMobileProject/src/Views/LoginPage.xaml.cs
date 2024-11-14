using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.ReCaptcha;
using UniMobileProject.src.Services.Validation;

namespace UniMobileProject.src.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly BasicAuthService _authService;
        private readonly ValidationService _validationService;

        private string _captchaToken = "";
        private bool isForgotPasswordMode = false;
        public LoginPage(BasicAuthService authService, ValidationService validationService)
        {
            InitializeComponent();
            _authService = authService;
            _validationService = validationService;

            CaptchaWebView.Source = new HtmlWebViewSource
            {
                BaseUrl = ReCaptchaService.baseUrl,
                Html = ReCaptchaService.captchaHtml
            };

            CaptchaWebView.Navigated += OnCaptchaNavigated;
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var email = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            // Валідація електронної пошти та пароля
            string? validationError = ValidateLoginInputs(email, password);
            if (validationError != null)
            {
                await DisplayAlert("Error", validationError, "OK");
                return;
            }

            if (string.IsNullOrEmpty(_captchaToken))
            {
                await DisplayAlert("Error", "Please complete the reCAPTCHA.", "OK");
                return;
            }

            // Якщо валідація успішна, виконуємо аутентифікацію
            var model = new LoginModel(email: email, password: password);
            var response = await _authService.Login(model);

            if (response is SuccessfulAuth)
            {
                await DisplayAlert("Success", "Logged in successfully!", "OK");
            }
            else if (response is FailedAuth failedResponse)
            {
                string errors = string.Join('\n', failedResponse.Errors);
                await DisplayAlert("Error", errors, "OK");
            }
        }

        private async void OnNavigateToRegister(object sender, EventArgs e)
        {
            _captchaToken = String.Empty;
            await Navigation.PushAsync(new RegisterPage(_authService, _validationService));
            CaptchaWebView.IsVisible = true;
            CaptchaWebView.Source = new HtmlWebViewSource
            {
                BaseUrl = ReCaptchaService.baseUrl,
                Html = ReCaptchaService.captchaHtml
            };
        }

        private async void OnCaptchaNavigated(object sender, WebNavigatedEventArgs e)
        {
            _captchaToken = await ReCaptchaService.HandleCaptchaNavigation(e);

            if (!string.IsNullOrEmpty(_captchaToken))
            {
                CaptchaWebView.IsVisible = false;
                await DisplayAlert("Success", "reCAPTCHA verified!", "OK");
            }
        }

        private string? ValidateLoginInputs(string email, string password)
        {
            // Валідація електронної пошти
            var (isEmailValid, emailError) = _validationService.EmailValidation(email);
            if (!isEmailValid)
                return emailError;

            // Валідація пароля
            var (isPasswordValid, passwordError) = _validationService.PasswordValidation(password);
            return isPasswordValid ? null : passwordError;
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            isForgotPasswordMode = !isForgotPasswordMode;
            ForgotPasswordEmailEntry.IsVisible = isForgotPasswordMode;
            ForgotPasswordSubmitButton.IsVisible = isForgotPasswordMode;
        }

        private async void OnForgotPasswordSubmitClicked(object sender, EventArgs e)
        {
            string email = ForgotPasswordEmailEntry.Text;

            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Error", "Please enter your email address.", "OK");
                return;
            }

            bool isSuccess = await _authService.RequestPasswordReset(email);

            if (isSuccess)
            {
                await DisplayAlert("Success", "Password reset link has been sent to your email.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to send password reset link. Please try again.", "OK");
            }
        }

    }
}