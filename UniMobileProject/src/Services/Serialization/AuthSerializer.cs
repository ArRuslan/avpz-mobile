using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels.AuthModels;

namespace UniMobileProject.src.Services.Serialization
{
    public class AuthSerializer : ISerializer
    {
        public async Task<T> Deserialize<T>(string content)
        {
            AuthResponse? response;
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            if (typeof(T).Equals(typeof(SuccessfulAuth)))
            {
                response = await JsonSerializer.DeserializeAsync<SuccessfulAuth>(stream);
            }
            else if (typeof(T).Equals(typeof(FailedAuth)))
            {
                response = await JsonSerializer.DeserializeAsync<FailedAuth>(stream);
            }
            else
            {
                throw new InvalidOperationException("Unsupported type provided");
            }

            return (T)(object)response ?? 
                throw new ArgumentNullException($"Can't deserialize content of {nameof(content)}");
        }

        public StringContent Serialize<T>(T model)
        {
            var json = JsonSerializer.Serialize<T>(model);
            return new StringContent(json);
        }
    }
}
