using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UniMobileProject.src.Enums;

namespace UniMobileProject.src.Models.ServiceModels.ProfileModels
{
    public class ProfileModel : RequestResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;
        [JsonPropertyName("role")]
        public Role Role { get; set; }
        [JsonPropertyName("mfa_enabled")]
        public bool MfaEnabled { get; set; }

        public ProfileModel()
        {
            this.IsSuccess = true;
        }
    }
}
