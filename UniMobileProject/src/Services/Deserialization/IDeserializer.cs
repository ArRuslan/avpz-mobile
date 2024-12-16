namespace UniMobileProject.src.Services.Deserialization
{
    public interface IDeserializer
    {
        Task<T> Deserialize<T>(string content);
    }
}
