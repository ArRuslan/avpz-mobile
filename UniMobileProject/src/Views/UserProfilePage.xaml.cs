using System;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.PageServices.Profile;
using UniMobileProject.src.Services.Serialization;
using UniMobileProject.src.Services.Auth;
using System.Security.Cryptography;
using QRCoder;
using UniMobileProject.src.Models.ServiceModels.ProfileModels;

namespace UniMobileProject.src.Views
{
    public partial class UserProfilePage : ContentPage
    {
        private readonly ProfileService _profileService;
        private readonly BasicAuthService _authService;
        private bool _isMfaEnabled;
        private string _secretKey;

        public UserProfilePage()
        {
            InitializeComponent();

            var httpServiceFactory = new HttpServiceFactory();
            var serializationFactory = new SerializationFactory();

            _profileService = new ProfileService(httpServiceFactory, serializationFactory);
            _authService = new BasicAuthService(httpServiceFactory, serializationFactory);

            // Set MFA status
            _isMfaEnabled = false;
            UpdateMfaButtonText();
        }

        private void UpdateMfaButtonText()
        {
            ToggleMfaButton.Text = _isMfaEnabled ? "Disable MFA" : "Enable MFA";
        }

        private async void OnGenerateMfaCodeClicked(object sender, EventArgs e)
        {
            try
            {
                string userPassword = await PromptUserForPassword();

                _secretKey = GenerateSecretKey();

                string email = await GetUserEmail();

                await ShowMfaSetup(_secretKey, email);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnEnableMfaClicked(object sender, EventArgs e)
        {
            try
            {
                string mfaCode = MfaCodeEntry.Text;

                if (string.IsNullOrEmpty(mfaCode))
                {
                    await DisplayAlert("Error", "Please enter the 6-digit code from your authenticator app.", "OK");
                    return;
                }

                string userPassword = await PromptUserForPassword();
                if (!ValidateMfaData(userPassword, _secretKey, mfaCode))
                {
                    return;
                }

                var enableMfaModel = new EnableMfaModel(userPassword, _secretKey, mfaCode);
                var response = await _profileService.EnableMfa(enableMfaModel);

                if (response is SuccessfulAuth)
                {
                    _isMfaEnabled = true;
                    UpdateMfaButtonText();
                    await DisplayAlert("Success", "MFA has been enabled.", "OK");
                }
                else
                {
                    string errors = string.Join('\n', ((FailedAuth)response).Errors);
                    await DisplayAlert("Error", errors, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async Task<string> PromptUserForPassword()
        {
            string password = await DisplayPromptAsync("Password Required", "Please enter your password:", "OK", "Cancel", "Password", maxLength: 100, keyboard: Keyboard.Text);
            if (string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("Password is required to enable/disable MFA.");
            }
            return password;
        }
        private bool ValidateMfaData(string password, string key, string code)
        {
            if (string.IsNullOrEmpty(password))
            {
                DisplayAlert("Error", "Password cannot be empty.", "OK");
                return false;
            }

            if (string.IsNullOrEmpty(key) || !System.Text.RegularExpressions.Regex.IsMatch(key, @"^[A-Z0-9]{16}$"))
            {
                DisplayAlert("Error", "Key must be exactly 16 characters long, containing only uppercase letters and digits.", "OK");
                return false;
            }

            if (string.IsNullOrEmpty(code) || !System.Text.RegularExpressions.Regex.IsMatch(code, @"^\d{6}$"))
            {
                DisplayAlert("Error", "Code must be exactly 6 digits.", "OK");
                return false;
            }

            return true;
        }

        private string GenerateSecretKey()
        {
            return new string('A', 16);
        }


        private Stream GenerateQrCode(string content)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return new MemoryStream(qrCode.GetGraphic(20));
        }

        private async Task<string> GetUserEmail()
        {
            try
            {
                var profileResponse = await _profileService.GetProfileModel();
                if (profileResponse is ProfileModel profile && !string.IsNullOrEmpty(profile.Email))
                {
                    return profile.Email;
                }
                else
                {
                    await DisplayAlert("Error", "Email is missing in profile data. Please try again.", "OK");
                    throw new InvalidOperationException("Email is missing in profile data.");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while retrieving user email: {ex.Message}", "OK");
                return string.Empty;
            }
        }

        private async Task ShowMfaSetup(string secretKey, string email)
        {
            try
            {
                string otpauthUrl = $"otpauth://totp/{email}?secret={secretKey}&issuer=HHB";

                var qrCodeStream = GenerateQrCode(otpauthUrl);
                var qrCodeImage = ImageSource.FromStream(() => qrCodeStream);

                QrCodeImage.Source = qrCodeImage;
                await DisplayAlert("MFA Setup", $"Scan this QR Code with your authenticator app:\n\nKey: {secretKey}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while setting up MFA: {ex.Message}", "OK");
                throw;
            }
        }
    }
}