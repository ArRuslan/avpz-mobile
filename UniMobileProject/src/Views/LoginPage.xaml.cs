using Microsoft.Maui.ApplicationModel.Communication;
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
        private readonly TokenMaintainer _tokenMaintainer = new TokenMaintainer();

        private string _captchaToken = "";
        private bool isForgotPasswordMode = false;
        public LoginPage(BasicAuthService authService, ValidationService validationService)
        {
            InitializeComponent();
            _authService = authService;
            _validationService = validationService;
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

            // Створюємо та показуємо попап
            var captchaPopup = new ReCaptchaPopup();
            await Navigation.PushModalAsync(captchaPopup);

            // Очікуємо завершення попапу
            _captchaToken = await captchaPopup.CaptchaTokenCompletionSource.Task;

            await LoginUser();
        }

        private async Task LoginUser()
        {
            if (string.IsNullOrEmpty(_captchaToken))
            {
                await DisplayAlert("Error", "Failed to verify reCAPTCHA.", "OK");
                return;
            }

            var model = new LoginModel(email: UsernameEntry.Text, password: PasswordEntry.Text);
            var response = await _authService.Login(model);

            if (response is SuccessfulAuth)
            {
                SuccessfulAuth successfulAuthResponse = (SuccessfulAuth)response;
                bool success = await _tokenMaintainer.SetToken(successfulAuthResponse);
                if (!success)
                {
                    await DisplayAlert("Error", "Something went wrong during authorization, please try again later", "Ok");
                }
                else
                {
                    // Встановлюємо MainTabbedPage як нову кореневу сторінку
                    Application.Current.MainPage = new MainTabbedPage();
                }
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
        }

        private string? ValidateLoginInputs(string email, string password)
        {
            // Валідація електронної пошти
            var (isEmailValid, emailError) = _validationService.ValidateEmail(email);
            if (!isEmailValid)
                return emailError;

            // Валідація пароля
            var (isPasswordValid, passwordError) = _validationService.ValidatePassword(password);
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