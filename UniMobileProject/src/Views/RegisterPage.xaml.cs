using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Validation;
using UniMobileProject.src.Services.ReCaptcha;

namespace UniMobileProject.src.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly BasicAuthService _authService;
        private readonly ValidationService _validationService;
        private readonly TokenMaintainer _tokenMaintainer = new TokenMaintainer();

        private string _captchaToken = ""; 

        public RegisterPage(BasicAuthService authService, ValidationService validationService)
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

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            var email = UsernameEntry.Text;
            var password = PasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;
            var phoneNumber = PhoneNumberEntry.Text;

            string? validationError = ValidateInputs(email, password, confirmPassword, phoneNumber);
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

            // Якщо валідація пройдена, створюємо модель і викликаємо метод реєстрації
            var model = new RegisterModel(
                email: email,
                password: password,
                firstName: FirstNameEntry.Text,
                lastName: LastNameEntry.Text,
                phoneNumber: phoneNumber,
                captchaKey: _captchaToken
            );

            var response = await _authService.Register(model);

            if (response is SuccessfulAuth)
            {
                SuccessfulAuth succesfulAuthResponse = (SuccessfulAuth)response;
                bool success = await _tokenMaintainer.SetToken(succesfulAuthResponse);
                if (!success)
                {
                    await DisplayAlert("Error", "Something went wrong during authorization, please try again later", "Ok");
                }
                else
                {
                    await DisplayAlert("Success", "Registered successfully!", "OK");
                }
                //ClearFields();
                await Navigation.PopAsync();
            }
            else if (response is FailedAuth failedResponse)
            {
                string errors = string.Join('\n', failedResponse.Errors);
                await DisplayAlert("Error", errors, "OK");
            }
        }

        private string? ValidateInputs(string email, string password, string confirmPassword, string phoneNumber)
        {
            // Перевірка, чи паролі співпадають
            if (password != confirmPassword)
                return "Passwords do not match";

            // Валідація електронної пошти
            var (isEmailValid, emailError) = _validationService.ValidateEmail(email);
            if (!isEmailValid)
                return emailError;

            // Валідація пароля
            var (isPasswordValid, passwordError) = _validationService.ValidatePassword(password);
            if (!isPasswordValid)
                return passwordError;

            // Валідація номера телефону
            var (isPhoneNumberValid, phoneError) = _validationService.ValidatePhoneNumber(phoneNumber);
            return isPhoneNumberValid ? null : phoneError;
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

        private void ClearFields()
        {
            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;
            FirstNameEntry.Text = string.Empty;
            LastNameEntry.Text = string.Empty;
            PhoneNumberEntry.Text = string.Empty;
        }
    }
}