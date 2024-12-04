
using System.Text;
using System.Text.Json;

namespace UniMobileProject.src.Services.Serialization;

public class BookingSerializer : ISerializer
{
    public async Task<T> Deserialize<T>(string content)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return await JsonSerializer.DeserializeAsync<T>(stream)
               ?? throw new ArgumentNullException("Failed to deserialize content.");
    }

    public string Serialize<T>(T model)
    {
        return JsonSerializer.Serialize(model);
    }
}
