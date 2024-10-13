using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniMobileProject.src.Services.Http
{
    public interface IHttpServiceFactory
    {
        HttpService Create(string endpoint);
    }
}
