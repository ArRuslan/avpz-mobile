using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels.AuthModels;

namespace UniMobileProject.src.Services.Http
{
    public class HttpService
    {
        private Uri baseAddress;
        private readonly HttpClient _client;

        public HttpService(string endpoint)
        {
            baseAddress = new Uri($"https://ticketer.ruslan.page/api/{endpoint}/");

            _client = new HttpClient()
            {
                BaseAddress = baseAddress
            };
        }

        public HttpClient GetClient()
        {
            return _client;
        }
    }
}
