using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Validation;
using UniMobileProject.src.Services.ReCaptcha;
using Microsoft.Maui.ApplicationModel.Communication;
using PhoneNumbers;

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

            // ��������� �� �������� �����
            var captchaPopup = new ReCaptchaPopup();
            await Navigation.PushModalAsync(captchaPopup);

            // ������� ���������� ������
            _captchaToken = await captchaPopup.CaptchaTokenCompletionSource.Task;

            await RegisterUser();
        }

        private async Task RegisterUser()
        {
            if (string.IsNullOrEmpty(_captchaToken))
            {
                await DisplayAlert("Error", "Failed to verify reCAPTCHA.", "OK");
                return;
            }

            var model = new RegisterModel(
                email: UsernameEntry.Text,
                password: PasswordEntry.Text,
                firstName: FirstNameEntry.Text,
                lastName: LastNameEntry.Text,
                phoneNumber: PhoneNumberEntry.Text,
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
            // ��������, �� ����� ����������
            if (password != confirmPassword)
                return "Passwords do not match";

            // �������� ���������� �����
            var (isEmailValid, emailError) = _validationService.ValidateEmail(email);
            if (!isEmailValid)
                return emailError;

            // �������� ������
            var (isPasswordValid, passwordError) = _validationService.ValidatePassword(password);
            if (!isPasswordValid)
                return passwordError;

            // �������� ������ ��������
            var (isPhoneNumberValid, phoneError) = _validationService.ValidatePhoneNumber(phoneNumber);
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