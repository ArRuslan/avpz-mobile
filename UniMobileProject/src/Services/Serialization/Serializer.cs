using System.Text.Json;

namespace UniMobileProject.src.Services.Serialization; 
public static class Serializer
{
    public static string Serialize<T>(T model)
    {
        var json = JsonSerializer.Serialize<T>(model);
        return json;
    }
}
