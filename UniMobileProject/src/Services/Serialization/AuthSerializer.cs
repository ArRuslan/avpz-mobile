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
                string errorMessage = ExtractErrorMessage(content);
                if (errorMessage == string.Empty) throw new ArgumentException("Program wasn't able" +
                    "to get error message from json");
                response = new FailedAuth() { ResponseContent = errorMessage };
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

        private string ExtractErrorMessage(string jsonContent)
        {
            using (JsonDocument document = JsonDocument.Parse(jsonContent))
            {
                JsonElement root = document.RootElement;
                if(root.TryGetProperty("detail", out JsonElement detailEleement) &&
                    detailEleement.ValueKind == JsonValueKind.Array &&
                    detailEleement.GetArrayLength() > 0)
                {
                    JsonElement first = detailEleement[0];

                    if(first.TryGetProperty("msg", out JsonElement msgElement))
                    {
                        return msgElement.ToString() ?? string.Empty;
                    }
                }
            }
            return string.Empty;
        }
    }
}
