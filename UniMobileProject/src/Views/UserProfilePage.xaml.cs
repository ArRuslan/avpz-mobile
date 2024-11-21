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
using System.Xml;

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

            // Завантажуємо профіль при запуску сторінки
            Task.Run(async () => await LoadProfile());
        }

        private async Task CheckMfaStatus()
    {
        try
        {
            var profileResponse = await _profileService.GetProfileModel();
            if (profileResponse is ProfileModel profile)
            {
                _isMfaEnabled = profile.MfaEnabled;
                UpdateMfaButtonText(); // Обновляем текст кнопки в зависимости от состояния MFA
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred while checking MFA status: {ex.Message}", "OK");
        }
    }

    // Обновление текста кнопки в зависимости от состояния MFA
    private void UpdateMfaButtonText()
    {
        ToggleMfaButton.Text = _isMfaEnabled ? "Disable MFA" : "Enable MFA";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CheckMfaStatus(); // Проверяем состояние MFA при загрузке страницы
    }

        private async void OnGenerateMfaCodeClicked(object sender, EventArgs e)
        {
            try
            {

                _secretKey = GenerateSecretKey();

                string email = await GetUserEmail();

                await ShowMfaSetup(_secretKey, email);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnToggleMfaClicked(object sender, EventArgs e)
        {
            try
            {
                string userPassword = await PromptUserForPassword();
                string mfaCode = MfaCodeEntry.Text;

                if (_isMfaEnabled)
                {
                    // Отключение MFA
                    var disableMfaModel = new DisableMfaModel(userPassword, mfaCode);
                    var response = await _profileService.DisableMfa(disableMfaModel);

                    if (response is ProfileModel profileResponse)
                    {
                        _isMfaEnabled = profileResponse.MfaEnabled; // false после отключения
                        UpdateMfaButtonText();
                        await DisplayAlert("Success", "MFA has been disabled.", "OK");
                    }
                    else if (response is FailedAuth failedAuthResponse)
                    {
                        string errors = string.Join('\n', failedAuthResponse.Errors);
                        await DisplayAlert("Error", errors, "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Unexpected response type.", "OK");
                    }
                }
                else
                {
                    // Включение MFA

                    if (string.IsNullOrEmpty(mfaCode))
                    {
                        await DisplayAlert("Error", "Please enter the 6-digit code from your authenticator app.", "OK");
                        return;
                    }

                    if (!ValidateMfaData(userPassword, _secretKey, mfaCode))
                    {
                        return;
                    }

                    var enableMfaModel = new EnableMfaModel(userPassword, _secretKey, mfaCode);
                    var response = await _profileService.EnableMfa(enableMfaModel);

                    if (response is ProfileModel profileResponse)
                    {
                        _isMfaEnabled = profileResponse.MfaEnabled; // true после включения
                        UpdateMfaButtonText();
                        await DisplayAlert("Success", "MFA has been enabled.", "OK");
                    }
                    else if (response is FailedAuth failedAuthResponse)
                    {
                        string errors = string.Join('\n', failedAuthResponse.Errors);
                        await DisplayAlert("Error", errors, "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Unexpected response type.", "OK");
                    }
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

        private async Task LoadProfile()
        {
            try
            {
                var response = await _profileService.GetProfileModel();
                if (response == null)
                {
                    // Token expired
                    ShowError("Your session has expired. Please log in again.");
                    return;
                }

                if (response is ProfileModel profile)
                {
                    FirstNameLabel.Text = profile.FirstName;
                    LastNameLabel.Text = profile.LastName;
                    PhoneLabel.Text = profile.PhoneNumber;
                    EmailLabel.Text = profile.Email;
                    ProfileErrorMessage.IsVisible = false;
                }
                else if (response is ErrorResponse error)
                {
                    // Відображаємо помилки
                    ProfileErrorMessage.Text = string.Join("\n", error.Errors);
                    ProfileErrorMessage.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred: {ex.Message}");
            }
        }

        private void OnEditButtonClicked(object sender, EventArgs e)
        {
            // Перемикаємо видимість між режимами перегляду та редагування
            ProfileMode.Text = "Edit Profile";
            EditButton.IsVisible = false;
            DisplayMode.IsVisible = false;
            EditMode.IsVisible = true;

            // Заповнюємо поля введення існуючими значеннями
            FirstNameEntry.Text = FirstNameLabel.Text;
            LastNameEntry.Text = LastNameLabel.Text;
            PhoneNumberEntry.Text = PhoneLabel.Text;
            EmailLabelEntry.Text = EmailLabel.Text;
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                // Створюємо модель для оновлення профілю
                var editProfileModel = new EditProfileModel
                {
                    FirstName = FirstNameEntry.Text,
                    LastName = LastNameEntry.Text,
                    PhoneNumber = PhoneNumberEntry.Text
                };

                // Відправляємо запит на оновлення профілю
                var response = await _profileService.UpdateProfile(editProfileModel);

                if (response is ProfileModel updatedProfile)
                {
                    // Оновлюємо відображення профілю
                    FirstNameLabel.Text = updatedProfile.FirstName;
                    LastNameLabel.Text = updatedProfile.LastName;
                    PhoneLabel.Text = updatedProfile.PhoneNumber;

                    // Повертаємося до режиму перегляду
                    ProfileMode.Text = "Profile Info";
                    EditButton.IsVisible = true;
                    DisplayMode.IsVisible = true;
                    EditMode.IsVisible = false;

                    ProfileErrorMessage.IsVisible = false;
                }
                else if (response is ErrorResponse error)
                {
                    // Відображаємо помилки
                    ProfileErrorMessage.Text = string.Join("\n", error.Errors);
                    ProfileErrorMessage.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                //ShowError($"An error occurred: {ex.Message}");
                ShowError("Phone number is not valid");
            }
        }

        private void ShowError(string error)
        {
            ProfileErrorMessage.Text = error;
            ProfileErrorMessage.IsVisible = true;
        }
    }
}