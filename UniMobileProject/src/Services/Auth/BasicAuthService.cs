using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniMobileProject.src.Services.Auth
{
    public class BasicAuthService
    {
        private HttpClient _client;
        public BasicAuthService()
        {
            _client = new HttpClient();
        }
    }
}
