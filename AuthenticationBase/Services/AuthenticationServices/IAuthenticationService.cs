using AuthenticationBase.Services.Result;
using System.Security.Claims;

namespace AuthenticationBase.Services.AuthenticationServices;

public interface IAuthenticationService
{
    Task<ResultService<string>> SignIn(string login, string password);
    Task<ResultService<string>> SignUp(string fullName, string login, string password);
    Task<ResultService<(ClaimsPrincipal, string)>> Refresh(string accessToken);
    Task SignOut();
}