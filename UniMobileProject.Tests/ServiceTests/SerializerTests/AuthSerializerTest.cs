using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels.AuthModels;
using UniMobileProject.src.Services.Serialization;

namespace UniMobileProject.Tests.ServiceTests.SerializerTests
{
    public class AuthSerializerTest
    {
        private ISerializationFactory _factory;
        private ISerializer serializer;
        private List<(string, SuccessfulAuth)> successfulJsonRequestDeserialize = new List<(string, SuccessfulAuth)>()
        {
            new ("{\"token\": \"tokendata\"}", new SuccessfulAuth(){ResponseContent = "tokendata"})
        };

        private List<(string, FailedAuth)> failedJsonRequestDeserialize = new List<(string, FailedAuth)>()
        {
            new ("{\"detail\": [" +
                "{" +
                "\"loc\": [" +
                "\"string\"," +
                "0" +
                "]," +
                "\"msg\": \"error\"," +
                "\"type\": \"string\"" +
                "}" +
                "]}", new FailedAuth() {ResponseContent = "error"})
        };

        private List<(LoginModel, string)> loginModelSerializationData = new List<(LoginModel, string)>()
        {
            new (new LoginModel("user@example.com", "string"), "{\"email\":\"user@example.com\"," +
                "\"password\":\"string\"}")
        };

        public AuthSerializerTest()
        {
            _factory = new SerializationFactory();
            serializer = _factory.Create(src.Enums.SerializerType.Auth);
        }

        [Fact]
        private async void DeserializeSuccessfulRequest_CorrectData_True()
        {
            foreach(var (input, expected) in successfulJsonRequestDeserialize)
            {
                var actual = await serializer.Deserialize<SuccessfulAuth>(input);
                Assert.Equal(expected.ResponseContent, actual.ResponseContent);
            }
        }

        [Fact]
        private async void DeserializeFailedRequest_CorrectData_True()
        {
            foreach(var (input, expected) in failedJsonRequestDeserialize)
            {
                var actual = await serializer.Deserialize<FailedAuth>(input);
                Assert.Equal(expected.ResponseContent, actual.ResponseContent);
            }
        }

        [Fact]
        private async void SerializeLoginModel_CorrectData_True()
        {
            foreach(var (input, expected) in loginModelSerializationData)
            {
                var actual = await serializer.Serialize<LoginModel>(input).ReadAsStringAsync();
                Assert.Equal(expected, actual);
            }
        }
    }
}
