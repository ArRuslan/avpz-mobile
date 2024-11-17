using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class DisableMfaModel
    {
        public string Password { get; set; }
        public string Code { get; set; }

        public DisableMfaModel(string password, string code)
        {
            Password = password;
            Code = code;
        }
    }

}
