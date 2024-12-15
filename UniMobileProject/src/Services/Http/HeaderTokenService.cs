using UniMobileProject.src.Services.Auth;
using UniMobileProject.src.Services.Database.Models;

namespace UniMobileProject.src.Services.Http;

public static class HeaderTokenService
{
    public static bool AddTokenToHeader(TokenMaintainer tokenMaintainer, ref HttpClient httpClient)
    {
        Token? token = tokenMaintainer.GetToken().Result;
        if (token == null) return false;
        var currentTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (token.ExpiresAtTimeSpan < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            return false;
        }
        if (httpClient.DefaultRequestHeaders.Contains("x-token"))
        {
            var header = httpClient.DefaultRequestHeaders.First(a => a.Key == "x-token");
            if (header.Value.Any(a => a == token.TokenString))
            {
                return true;
            }
            else
            {
                httpClient.DefaultRequestHeaders.Remove("x-token");
                httpClient.DefaultRequestHeaders.Add("x-token", token.TokenString);
                return true;
            }
        }
        httpClient.DefaultRequestHeaders.Add("x-token", token.TokenString);
        return true;
    }
}
