using UniMobileProject.src.Enums;

namespace UniMobileProject.src.Services.Deserialization
{
    public interface IDeserializationFactory
    {
        IDeserializer Create(DeserializerType type);
    }
}