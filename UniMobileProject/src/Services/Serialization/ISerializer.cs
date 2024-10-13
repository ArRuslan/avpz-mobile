using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniMobileProject.src.Services.Serialization
{
    public interface ISerializer
    {
        StringContent Serialize<T>(T model);
        Task<T> Deserialize<T>(string content);
    }
}
