using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Database;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Serialization;
using UniMobileProject.src.Services.Validation;
using UniMobileProject.src.Views;

namespace UniMobileProject
{
    public partial class App : Application
    {
        public App()
        {
            var httpServiceFactory = new HttpServiceFactory();
            var serializationFactory = new SerializationFactory();

            BasicAuthService authService = new BasicAuthService(httpServiceFactory, serializationFactory);

            var validationService = new ValidationService();
            var dbService = new DatabaseService();

            MainPage = new NavigationPage(new LoginPage(authService, validationService));
        }
    }
}
