using System.Text.Json.Serialization;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class SuccessfulAuth : AuthResponse
    {
        [JsonPropertyName("token")]
        public string ResponseContent { get; set; } = string.Empty;
        public SuccessfulAuth()
        {
            this.IsSuccess = true;
        }
    }
}
