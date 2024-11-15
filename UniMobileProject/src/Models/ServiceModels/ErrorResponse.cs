using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UniMobileProject.src.Models.ServiceModels
{
    internal class ErrorResponse : RequestResponse
    {
        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = new List<string>();
        public ErrorResponse()
        {
            this.IsSuccess = false;
        }
    }
}
