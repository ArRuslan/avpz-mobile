using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.Tests.ServiceTests.AuthTests
{
    public class BasicAuthTest
    {
        private IHttpServiceFactory _httpFactory;
        private ISerializationFactory _serializationFactory;
        private BasicAuthService service;

        public BasicAuthTest()
        {
            _httpFactory = new HttpServiceFactory();
            _serializationFactory = new SerializationFactory();
            service = new BasicAuthService(_httpFactory, _serializationFactory);
        }
    }
}
