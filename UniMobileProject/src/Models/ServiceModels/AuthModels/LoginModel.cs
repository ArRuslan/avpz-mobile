using System.Text.Json.Serialization;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class LoginModel
    {
        [JsonPropertyName("email")]
        public string Email { get; }
        [JsonPropertyName("password")]
        public string Password { get; }

        public LoginModel(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
