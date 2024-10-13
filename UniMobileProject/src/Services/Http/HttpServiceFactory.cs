using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniMobileProject.src.Services.Http
{
    public class HttpServiceFactory : IHttpServiceFactory
    {
        public HttpService Create(string endpoint)
        {
            return new HttpService(endpoint);
        }
    }
}
