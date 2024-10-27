using System.Text.Json.Serialization;
using UniMobileProject.src.Enums;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class RegisterModel
    {
        [JsonPropertyName("captcha_key")]
        public string CaptchaKey { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; }
        [JsonPropertyName("password")]
        public string Password { get; }
        [JsonPropertyName("first_name")]
        public string FirstName { get; }
        [JsonPropertyName("last_name")]
        public string LastName { get; }
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; }
        [JsonPropertyName("role")]
        public Role Role { get; }

        public RegisterModel(string email, string password, string firstName,
            string lastName, string phoneNumber, string captchaKey = "somekey")
        {
            CaptchaKey = captchaKey;
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Role = Role.Customer;
        }

    }
}
