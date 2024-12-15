using UniMobileProject.Resources.Themes;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Database;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Deserialization;
using UniMobileProject.src.Services.Validation;
using UniMobileProject.src.Views;

namespace UniMobileProject
{
    public partial class App : Application
    {
        public App()
        {
            ApplySystemTheme();

            this.RequestedThemeChanged += OnRequestedThemeChanged;

            var httpServiceFactory = new HttpServiceFactory();
            var serializationFactory = new DeserializationFactory();

            BasicAuthService authService = new BasicAuthService(httpServiceFactory, serializationFactory);

            var validationService = new ValidationService();
            var dbService = new DatabaseService();

            MainPage = new NavigationPage(new LoginPage(authService, validationService));
        }

        private void ApplySystemTheme()
        {
            var currentTheme = Application.Current.RequestedTheme;

            if (currentTheme == AppTheme.Dark)
            {
                Resources.MergedDictionaries.Clear();
                Resources.MergedDictionaries.Add(new DarkTheme());
            }
            else
            {
                Resources.MergedDictionaries.Clear();
                Resources.MergedDictionaries.Add(new LightTheme());
            }
        }

        private void OnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            // Оновлення теми при зміні системної
            ApplySystemTheme();
        }
    }
}
