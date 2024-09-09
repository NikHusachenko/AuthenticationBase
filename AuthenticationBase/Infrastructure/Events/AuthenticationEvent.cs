using AuthenticationBase.Services.AuthenticationServices;
using AuthenticationBase.Services.Result;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace AuthenticationBase.Infrastructure.Events;

public sealed class AuthenticationEvent : JwtBearerEvents
{
    private const string RefreshTokenCookieName = "Token";
    private const string AuthenticationHeaderName = "Authorization";
    private const string UnauthorizedError = "Unauthorized";

    public override async Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        using IServiceScope scope = context.HttpContext.RequestServices.CreateScope();
        IAuthenticationService authenticationService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

        string? refreshToken = context.HttpContext.Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            OnFail(context);
            return;
        }

        ResultService<(ClaimsPrincipal, string)> refreshResult = await authenticationService.Refresh(refreshToken);
        if (refreshResult.IsError)
        {
            OnFail(context);
            return;
        }

        OnSuccess(context, refreshResult.Result.Item1, refreshResult.Result.Item2);
    }

    private void OnFail(AuthenticationFailedContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.HttpContext.User = new ClaimsPrincipal();
        context.Principal = null;
        context.Fail(UnauthorizedError);
    }

    private void OnSuccess(AuthenticationFailedContext context, ClaimsPrincipal principal, string token)
    {
        context.Response.Cookies.Append(AuthenticationHeaderName, $"Bearer {token}");
        context.Response.StatusCode = 200;
        context.Principal = principal;
        context.HttpContext.User = principal;
        context.Success();
    }
}