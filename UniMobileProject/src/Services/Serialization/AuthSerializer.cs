using System.Text;
using System.Text.Json;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.AuthModels;

namespace UniMobileProject.src.Services.Serialization
{
    public class AuthSerializer : ISerializer
    {
        public async Task<T> Deserialize<T>(string content)
        {
            RequestResponse? response;
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

        public string Serialize<T>(T model)
        {
            var json = JsonSerializer.Serialize<T>(model);
            return json;
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
