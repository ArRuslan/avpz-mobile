using System.Text;
using System.Text.Json;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.ProfileModels;

namespace UniMobileProject.src.Services.Deserialization;

public class ProfileDeserializer : IDeserializer
{
    public async Task<T> Deserialize<T>(string content)
    {
        RequestResponse? response;
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        if (typeof(T).Equals(typeof(ProfileModel)))
        {
            response = await JsonSerializer.DeserializeAsync<ProfileModel>(stream);
        }
        else if (typeof(T).Equals(typeof(ErrorResponse)))
        {
            response = await JsonSerializer.DeserializeAsync<ErrorResponse>(stream);
        }
        else
        {
            throw new InvalidOperationException("Unsupported type provided");
        }
        return (T)(object)response ?? 
            throw new ArgumentNullException($"Can't deserialize content of {nameof(content)}");
    }
}
