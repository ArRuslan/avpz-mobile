using Microsoft.Maui.ApplicationModel.Communication;
using UniMobileProject.src.Enums;
using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Models.ServiceModels.ProfileModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Deserialization;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.PageServices.Profile;
using UniMobileProject.src.Services.ReCaptcha;
using UniMobileProject.src.Services.Validation;

namespace UniMobileProject.src.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly BasicAuthService _authService;
        private readonly ValidationService _validationService;
        private readonly TokenMaintainer _tokenMaintainer = new TokenMaintainer();
        private readonly ProfileService _profileService;

        private string _captchaToken = "";
        private bool isForgotPasswordMode = false;
        public LoginPage(BasicAuthService authService, ValidationService validationService)
        {
            InitializeComponent();
            _authService = authService;
            _validationService = validationService;
            _profileService = new ProfileService(new HttpServiceFactory(), new DeserializationFactory());
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var email = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            string? validationError = ValidateLoginInputs(email, password);
            if (validationError != null)
            {
                await DisplayAlert("Error", validationError, "OK");
                return;
            }

            var captchaPopup = new ReCaptchaPopup();
            await Navigation.PushModalAsync(captchaPopup);

            // Очікуємо завершення попапу
            //_captchaToken = "123456789";
            _captchaToken = await captchaPopup.CaptchaTokenCompletionSource.Task;

            await LoginUser(email, password);
        }

        private async Task LoginUser(string email, string password)
        {
            if (string.IsNullOrEmpty(_captchaToken))
            {
                await DisplayAlert("Error", "Failed to verify reCAPTCHA.", "OK");
                return;
            }

            var model = new LoginModel(email, password);
            var response = await _authService.Login(model);

            if (response is SuccessfulAuth successfulAuthResponse)
            {
                if (successfulAuthResponse is MfaRequiredAuth mfaRequiredAuth)
                {
                    await Navigation.PushAsync(new MfaPage(_authService, mfaRequiredAuth.MfaToken));
                    return; 
                }

                // Збереження токену
                bool tokenSetSuccess = await _tokenMaintainer.SetToken(successfulAuthResponse);
                if (!tokenSetSuccess)
                {
                    await DisplayAlert("Error", "Something went wrong during authorization, please try again later", "Ok");
                    return;
                }

                // Завантаження профілю користувача
                var profileResponse = await _profileService.GetProfileModel();
                if (profileResponse is ProfileModel profileResult && profileResult.IsSuccess)
                {
                    var userRole = profileResult.Role;

                    // Перевірка ролі
                    if (userRole == Role.Admin)
                    {
                        bool isAdminConfirmed = await DisplayAlert(
                            "Admin Access",
                            "Do you want to join the admin panel?",
                            "Yes",
                            "No"
                        );

                        if (isAdminConfirmed)
                        {
                            Application.Current.MainPage = new NavigationPage(new AdminPage());
                            return;
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to load user profile.", "OK");
                }

                // Перехід на основну сторінку
                Application.Current.MainPage = new NavigationPage(new MainTabbedPage());
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