using System.Xml;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.ProfileModels;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.PageServices.Profile;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.src.Views;

public partial class UserProfilePage : ContentPage
{
    private readonly ProfileService _profileService;

    public UserProfilePage()
    {
        InitializeComponent();

        // Створюємо сервіси (це залежить від DI-контейнера, якщо є)
        var httpServiceFactory = new HttpServiceFactory();
        var serializationFactory = new SerializationFactory();

        _profileService = new ProfileService(httpServiceFactory, serializationFactory);

        // Завантажуємо профіль при запуску сторінки
        //Task.Run(async () => await LoadProfileAsync());
    }

    //private async Task LoadProfileAsync()
    //{
    //    try
    //    {
    //        var response = await _profileService.GetProfileModel();
    //        if (response == null)
    //        {
    //            // Token expired
    //            ShowError("Your session has expired. Please log in again.");
    //            return;
    //        }

    //        if (response.Data is ProfileModel profile)
    //        {
    //            NameLabel.Text = $"Name: {profile.Name}";
    //            EmailLabel.Text = $"Email: {profile.Email}";
    //            ErrorMessage.IsVisible = false;
    //        }
    //        else if (response.Data is ErrorResponse error)
    //        {
    //            ShowError(error.Message);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ShowError("An unexpected error occurred.");
    //    }
    //}

    //private void ShowError(string message)
    //{
    //    ErrorMessage.Text = message;
    //    ErrorMessage.IsVisible = true;
    //}

    //private async void OnRefreshProfileClicked(object sender, EventArgs e)
    //{
    //    await LoadProfileAsync();
    //}
}