using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Models.ServiceModels.ProfileModels;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.PageServices.Profile;
using UniMobileProject.src.Services.Deserialization;

namespace UniMobileProject.Tests.ServiceTests.ProfileTests
{
    public class ProfileTest
    {
        private IHttpServiceFactory _httpFactory;
        private IDeserializationFactory _serializationFactory;
        private BasicAuthService authService;
        private ProfileService profileService;
        private TokenMaintainer _tokenMaintainer;

        LoginModel model = new LoginModel("email2@gmail.com", "Password12_");

        public ProfileTest()
        {
            _httpFactory = new HttpServiceFactory();
            _serializationFactory = new DeserializationFactory();
            authService = new BasicAuthService(_httpFactory, _serializationFactory);
            profileService = new ProfileService(_httpFactory, _serializationFactory, "mytestdb.db");
            _tokenMaintainer = new TokenMaintainer("mytestdb.db");
        }

        [Fact]
        public async void GetProfile_ProfileModel_Correct()
        {
            var isLoggedIn = await Login();
            Assert.True(isLoggedIn);
            RequestResponse? response = await profileService.GetProfileModel();
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            ProfileModel model = (ProfileModel)response;
            Assert.NotNull(model);
        }

        private async Task<bool> Login()
        {
            RequestResponse response = await authService.Login(model);
            if (response.IsSuccess)
            {
                bool isSuccess = await _tokenMaintainer.SetToken((SuccessfulAuth)response);
                return isSuccess;
            }
            return false;
        }

        [Fact]
        public async void UpdateProfile_RequestResponse_Correct()
        {
            var isLoggedIn = await Login();
            Assert.True(isLoggedIn);
            RequestResponse? response = await profileService.GetProfileModel();
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            ProfileModel model = (ProfileModel)response;
            Assert.NotNull(model);
            EditProfileModel editModel = new EditProfileModel { FirstName = "Johny", LastName = "Doe", PhoneNumber = model.PhoneNumber };
            RequestResponse? editResponse = await profileService.UpdateProfile(editModel);
            Assert.NotNull(editResponse);
        }
    }
}
