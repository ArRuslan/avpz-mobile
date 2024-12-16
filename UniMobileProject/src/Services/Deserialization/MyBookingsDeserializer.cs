
using System.Text;
using System.Text.Json;

namespace UniMobileProject.src.Services.Deserialization;

public class MyBookingsDeserializer : IDeserializer
{
    public async Task<T> Deserialize<T>(string content)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return await JsonSerializer.DeserializeAsync<T>(stream)
               ?? throw new ArgumentNullException("Failed to deserialize content.");
    }
}
