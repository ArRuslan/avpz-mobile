using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Auth;

namespace UniMobileProject.src.Views;

public partial class MfaPage : ContentPage
{
    private readonly BasicAuthService _authService;
    private readonly string _mfaToken;

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

        var mfaModel = new MfaLoginModel(_mfaToken, mfaCode);

        var response = await _authService.LoginWithMfa(mfaModel);

        if (response.IsSuccess)
        {
            await DisplayAlert("Success", "You have successfully logged in!", "OK");
            await Navigation.PushAsync(new MainPage());
        }
        else
        {
            await DisplayAlert("Error", "Invalid MFA code. Please try again.", "OK");
        }
    }

}
