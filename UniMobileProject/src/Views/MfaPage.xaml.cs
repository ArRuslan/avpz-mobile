using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Auth;

namespace UniMobileProject.src.Views;

public partial class MfaPage : ContentPage
{
    private readonly BasicAuthService _authService;
    private readonly string _mfaToken;
    private readonly TokenMaintainer _tokenMaintainer = new TokenMaintainer(); // Сервис для хранения токена

    public MfaPage(BasicAuthService authService, string mfaToken)
    {
        InitializeComponent();
        _authService = authService;
        _mfaToken = mfaToken;
    }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
        string mfaCode = MfaCodeEntry.Text;

        if (string.IsNullOrWhiteSpace(mfaCode) || mfaCode.Length != 6)
        {
            await DisplayAlert("Error", "Please enter a valid 6-digit code.", "OK");
            return;
        }

        var mfaModel = new MfaLoginModel(mfaCode, _mfaToken);

        // Получаем ответ от сервиса авторизации
        var response = await _authService.LoginWithMfa(mfaModel);

        // Проверяем, является ли response объектом SuccessfulAuth
        if (response is SuccessfulAuth successfulAuthResponse && successfulAuthResponse.IsSuccess)
        {
            // Сохраняем токен после успешного входа
            bool tokenSaved = await _tokenMaintainer.SetToken(successfulAuthResponse);
            if (!tokenSaved)
            {
                await DisplayAlert("Error", "Something went wrong while saving the token. Please try again.", "OK");
                return;
            }

            await DisplayAlert("Success", "You have successfully logged in!", "OK");
            Application.Current.MainPage = new MainTabbedPage();
        }
        else
            {
                await DisplayAlert("Error", "Invalid MFA code. Please try again.", "OK");
            }
        }

}

