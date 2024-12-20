using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels;
using UniMobileProject.src.Models.ServiceModels.AdminModels;

namespace UniMobileProject.src.Services.Deserialization
{
    public class AdminDeserializer : IDeserializer
    {
        public async Task<T> Deserialize<T>(string content)
        {
            using var jsonDocument = JsonDocument.Parse(content);
            JsonElement root = jsonDocument.RootElement;
            if (typeof(T).Equals(typeof(RoomSuccessModel)))
            {
                if (root.TryGetProperty("room", out JsonElement roomElement))
                {
                    var roomStream = new MemoryStream(Encoding.UTF8.GetBytes(roomElement.GetRawText()));
                    T response = await JsonSerializer.DeserializeAsync<T>(roomStream);
                    return response;
                }
                return default;
            }
            else
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                T response = await JsonSerializer.DeserializeAsync<T>(stream);
                return response;
            }
        }
    }
}
