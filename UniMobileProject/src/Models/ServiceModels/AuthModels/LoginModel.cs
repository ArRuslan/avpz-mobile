using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class LoginModel
    {
        public required string EmailOrName { get; set; }
        public required string Password { get; set; }
        public bool isEmail { get; set; }
    }
}
