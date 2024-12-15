using System.Text;
using System.Text.Json;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.AuthModels;

namespace UniMobileProject.src.Services.Deserialization
{
    public class AuthDeserializer : IDeserializer
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
    }
}
