using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class EnableMfaModel
    {
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        public EnableMfaModel(string password, string key, string code)
        {
            Password = password;
            Key = key;
            Code = code;
        }
    }

}
