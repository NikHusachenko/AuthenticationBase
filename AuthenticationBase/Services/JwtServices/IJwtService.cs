using AuthenticationBase.Services.Result;
using System.Security.Claims;

namespace AuthenticationBase.Services.JwtServices;

public interface IJwtService
{
    Task<ResultService<string>> GetRefreshToken(IEnumerable<Claim> claims);
    ResultService<string> GetAccessToken(IEnumerable<Claim> claims);
    ResultService<IEnumerable<Claim>> Decode(string token);

    Task<ResultService<ClaimsPrincipal>> VerifyRefreshToken(string token);
    Task<ResultService> DisableRefreshToken(string token);
}