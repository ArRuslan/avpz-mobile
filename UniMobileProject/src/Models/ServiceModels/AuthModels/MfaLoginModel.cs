using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class MfaLoginModel
    {
        [JsonPropertyName("mfa_code")]
        public string MfaCode { get; set; }
        [JsonPropertyName("mfa_token")]
        public string MfaToken { get; set; }

        public MfaLoginModel() { }
        public MfaLoginModel(string mfaCode, string mfaToken)
        {
            MfaCode = mfaCode;
            MfaToken = mfaToken;
        }
    }

}
