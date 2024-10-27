namespace UniMobileProject.src.Services.Serialization
{
    public interface ISerializer
    {
        string Serialize<T>(T model);
        Task<T> Deserialize<T>(string content);
    }
}
