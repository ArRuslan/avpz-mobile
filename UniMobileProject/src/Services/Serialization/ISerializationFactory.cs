using UniMobileProject.src.Enums;

namespace UniMobileProject.src.Services.Serialization
{
    public interface ISerializationFactory
    {
        ISerializer Create(SerializerType type);
    }
}