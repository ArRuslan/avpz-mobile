using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Validation;

namespace UniMobileProject.src.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly BasicAuthService _authService;
        private readonly ValidationService _validationService;

        public RegisterPage(BasicAuthService authService, ValidationService validationService)
        {
            InitializeComponent();
            _authService = authService;
            _validationService = validationService;
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

            // Якщо валідація пройдена, створюємо модель і викликаємо метод реєстрації
            var model = new RegisterModel(
                email: email,
                password: password,
                firstName: FirstNameEntry.Text,
                lastName: LastNameEntry.Text,
                phoneNumber: phoneNumber,
                captchaKey: "your_captcha_key_here"
            );

            var response = await _authService.Register(model);

            if (response is SuccessfulAuth)
            {
                await DisplayAlert("Success", "Registered successfully!", "OK");
                //ClearFields();
                await Navigation.PopAsync();
            }
            else if (response is FailedAuth failedResponse)
            {
                await DisplayAlert("Error", failedResponse.ResponseContent, "OK");
            }
        }

        private string? ValidateInputs(string email, string password, string confirmPassword, string phoneNumber)
        {
            // Перевірка, чи паролі співпадають
            if (password != confirmPassword)
                return "Passwords do not match";

            // Валідація електронної пошти
            var (isEmailValid, emailError) = _validationService.EmailValidation(email);
            if (!isEmailValid)
                return emailError;

            // Валідація пароля
            var (isPasswordValid, passwordError) = _validationService.PasswordValidation(password);
            if (!isPasswordValid)
                return passwordError;

            // Валідація номера телефону
            var (isPhoneNumberValid, phoneError) = _validationService.PhoneNumberValidation(phoneNumber);
            return isPhoneNumberValid ? null : phoneError;
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
