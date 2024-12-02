using UniMobileProject.src.Enums;

namespace UniMobileProject.src.Services.Serialization
{
    public class SerializationFactory : ISerializationFactory
    {
        public ISerializer Create(SerializerType type)
        {
            if (!Enum.IsDefined(typeof(SerializerType), type))
            {
                throw new ArgumentException($"Invalid SerializerType provided: {type}");
            }

            return type switch
            {
                SerializerType.Auth => new AuthSerializer(),
                SerializerType.Profile => new ProfileSerializer(),
                SerializerType.Hotel => new HotelSerializer(),
                _ => throw new ArgumentException($"Can't create a serializer with an unspecified type: {type}")
            };
        }

    }
}
