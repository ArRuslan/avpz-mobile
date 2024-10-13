using System.Text.Json.Serialization;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class FailedAuth : AuthResponse
    {
        [JsonPropertyName("msg")]
        public string ResponseContent { get; set; } = string.Empty;
        public FailedAuth()
        {
            this.IsSuccess = false;
        }
    }
}
