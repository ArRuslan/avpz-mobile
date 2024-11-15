using System.Text.Json.Serialization;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class FailedAuth : RequestResponse
    {
        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = new List<string>();
        public FailedAuth()
        {
            this.IsSuccess = false;
        }
    }
}
