using UniMobileProject.src.Enums;

namespace UniMobileProject.src.Services.Deserialization
{
    public class DeserializationFactory : IDeserializationFactory
    {
        public IDeserializer Create(DeserializerType type)
        {
            if (!Enum.IsDefined(typeof(DeserializerType), type))
            {
                throw new ArgumentException($"Invalid SerializerType provided: {type}");
            }

            return type switch
            {
                DeserializerType.Auth => new AuthDeserializer(),
                DeserializerType.Profile => new ProfileDeserializer(),
                DeserializerType.Hotel => new HotelDeserializer(),
                DeserializerType.Room => new RoomDeserializer(),
                DeserializerType.Booking => new BookingDeserializer(),
                DeserializerType.MyBookings => new MyBookingsDeserializer(),
                DeserializerType.Admin => new AdminDeserializer(),
                _ => throw new ArgumentException($"Can't create a serializer with an unspecified type: {type}")
            };
        }

    }
}
