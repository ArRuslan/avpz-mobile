using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Validation;

namespace UniMobileProject.src.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly BasicAuthService _authService;
        private readonly ValidationService _validationService;

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

            // Якщо валідація успішна, виконуємо аутентифікацію
            var model = new LoginModel(email: email, password: password);
            var response = await _authService.Login(model);

            if (response is SuccessfulAuth)
            {
                await DisplayAlert("Success", "Logged in successfully!", "OK");
            }
            else if (response is FailedAuth failedResponse)
            {
                await DisplayAlert("Error", failedResponse.ResponseContent, "OK");
            }
        }

        private async void OnNavigateToRegister(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage(_authService, _validationService));
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
    }
}