using System.Text.Json.Serialization;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class SuccessfulAuth : RequestResponse
    {
        [JsonPropertyName("token")]
        public string ResponseContent { get; set; } = string.Empty;
        [JsonPropertyName("expires_at")]
        public long ExpiresAt { get; set; }
        public SuccessfulAuth()
        {
            this.IsSuccess = true;
        }
    }
}
