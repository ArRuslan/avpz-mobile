using UniMobileProject.src.Enums;

namespace UniMobileProject.src.Services.Serialization
{
    public class SerializationFactory : ISerializationFactory
    {
        public ISerializer Create(SerializerType type)
        {
            return type switch
            {
                SerializerType.Auth => new AuthSerializer(),
                SerializerType.Profile => new ProfileSerializer(),
                _ => throw new ArgumentException($"Can't create a serializer with an unspecified type: {nameof(type)}")
            };
        }
    }
}
