using AuthenticationBase.Services.AuthenticationServices;
using AuthenticationBase.Services.Result;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace AuthenticationBase.Infrastructure.Middleware;

public class AuthenticationProtectionMiddleware(RequestDelegate next)
{
    private const string RefreshTokenCookieName = "Token";
    private const string AuthenticationHeaderName = "Authorization";
    private const string UnauthorizedError = "Unauthorized";

    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            using IServiceScope scope = context.RequestServices.CreateScope();
            IAuthenticationService authenticationService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

            string? refreshToken = context.Request.Cookies[RefreshTokenCookieName];
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

            await OnSuccess(context, next, refreshResult.Result.Item1, refreshResult.Result.Item2);
        }
    }

    private void OnFail(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.User = new ClaimsPrincipal();
    }

    private Task OnSuccess(HttpContext context, RequestDelegate next, ClaimsPrincipal principal, string token)
    {
        context.Response.Cookies.Append(AuthenticationHeaderName, $"Bearer {token}");
        context.Response.StatusCode = 200;
        context.User = principal;
        return next(context);
    }
}