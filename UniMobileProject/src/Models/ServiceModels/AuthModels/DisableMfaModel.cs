using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class DisableMfaModel
    {
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        public DisableMfaModel(string password, string code)
        {
            Password = password;
            Code = code;
        }
    }

}
