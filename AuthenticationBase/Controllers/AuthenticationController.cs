using AuthenticationBase.ApiRequests.Authentication;
using AuthenticationBase.Services.AuthenticationServices;
using AuthenticationBase.Services.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationBase.Controllers;

[Route(AuthenticationControllerRoute)]
public class AuthenticationController(IAuthenticationService authenticationService)
    : BaseController
{
    [HttpPost(SignUpRoute)]
    public async Task<IActionResult> SignUp([FromBody] SignUpApiRequest request)
    {
        ResultService<string> signUpResult = await authenticationService.SignUp(request.FullName, request.Login, request.Password);
        if (signUpResult.IsError)
        {
            return await AsError(signUpResult.ErrorMessage);
        }
        return await AsSuccess(signUpResult.Result);
    }

    [Authorize]
    [HttpGet("auth-ping")]
    public async Task<IActionResult> AuthPing()
    {
        var a = HttpContext;
        return Ok();
    }

    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        var a = HttpContext;
        return Ok();
    }
}