using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Validation;

namespace UniMobileProject.src.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly BasicAuthService _authService;
        private readonly ValidationService _validationService;
        private readonly TokenMaintainer _tokenMaintainer = new TokenMaintainer();

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

            // �������� ���������� ����� �� ������
            string? validationError = ValidateLoginInputs(email, password);
            if (validationError != null)
            {
                await DisplayAlert("Error", validationError, "OK");
                return;
            }

            // ���� �������� ������, �������� ��������������
            var model = new LoginModel(email: email, password: password);
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
                    await DisplayAlert("Success", "Logged in successfully!", "OK");
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
            await Navigation.PushAsync(new RegisterPage(_authService, _validationService));
        }

        private string? ValidateLoginInputs(string email, string password)
        {
            // �������� ���������� �����
            var (isEmailValid, emailError) = _validationService.ValidateEmail(email);
            if (!isEmailValid)
                return emailError;

            // �������� ������
            var (isPasswordValid, passwordError) = _validationService.ValidatePassword(password);
            return isPasswordValid ? null : passwordError;
        }
    }
}