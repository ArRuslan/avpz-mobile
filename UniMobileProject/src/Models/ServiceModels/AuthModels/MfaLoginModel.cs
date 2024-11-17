using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniMobileProject.src.Models.ServiceModels.AuthModels
{
    public class MfaLoginModel
    {
        public string MfaCode { get; set; }
        public string MfaToken { get; set; }

        public MfaLoginModel() { }
        public MfaLoginModel(string mfaCode, string mfaToken)
        {
            MfaCode = mfaCode;
            MfaToken = mfaToken;
        }
    }

}
