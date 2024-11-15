using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniMobileProject.src.Services.Database;
using UniMobileProject.src.Services.Database.Models;

namespace UniMobileProject.Tests.ServiceTests.DatabaseTests
{
    public class DatabaseTest
    {
        DatabaseService _service = new DatabaseService("newTestDb.db");

        Token oldToken = new Token() { TokenString = "asdafsfdfasfasfas", ExpiresAtTimeSpan = DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
        Token newToken = new Token() {Id = 1, TokenString = "actualTokenString", ExpiresAtTimeSpan = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 2 };

        [Fact]
        public void AddToken_true()
        {
            _service.AddToken(oldToken);
        }

        [Fact]
        public void GetToken_true()
        {
            var token = _service.GetToken();
            Assert.NotNull(token);
        }

        [Fact]
        public async void UpdateToken_true()
        {
            var old = _service.GetToken();
            _service.UpdateToken(newToken);
            var n = await _service.GetToken();
            Assert.Equal("actualTokenString", n.TokenString);
        }
    }
}
