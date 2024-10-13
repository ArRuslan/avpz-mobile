using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                _ => throw new ArgumentException($"Can't create a serializer with an unspecified type: {nameof(type)}")
            };
        }
    }
}
