using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniMobileProject.src.Models.ServiceModels.AuthModels;

namespace UniMobileProject.src.Services.Database.Models
{
    public class Token
    {
        public Token()
        {
        }
        public Token(SuccessfulAuth authData)
        {
            TokenString = authData.ResponseContent;
            ExpiresAtTimeSpan = authData.ExpiresAt;
        }
        public int Id { get; set; }
        public string TokenString { get; set; } = string.Empty;
        public long ExpiresAtTimeSpan { get; set; }
    }
}
