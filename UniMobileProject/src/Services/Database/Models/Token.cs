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
            Id = 1;
        }
        public int Id { get; set; }
        public string TokenString { get; set; } = string.Empty;
        public long ExpiresAtTimeSpan { get; set; }
    }
}
