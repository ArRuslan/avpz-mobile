using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class MfaRequiredAuth : SuccessfulAuth
    {
        public string MfaToken { get; set; }
    }
}
