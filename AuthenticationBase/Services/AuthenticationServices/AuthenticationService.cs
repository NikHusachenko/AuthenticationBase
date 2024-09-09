using AuthenticationBase.EntityFramework.Entities;
using AuthenticationBase.EntityFramework.Enums;
using AuthenticationBase.EntityFramework.Repository;
using AuthenticationBase.Services.JwtServices;
using AuthenticationBase.Services.JwtServices.Options;
using AuthenticationBase.Services.Result;
using System.Security.Claims;

namespace AuthenticationBase.Services.AuthenticationServices;

public sealed class AuthenticationService(
    IHttpContextAccessor httpContextAccessor,
    IGenericRepository<UserEntity> userRepository,
    IJwtService jwtService)
    : IAuthenticationService
{
    private const string RefreshTokenCookieName = "Token";
    private const string InvalidCredentialsError = "Invalid credentials.";
    private const string WasCreatedError = "Was created.";
    private const string RegistrationError = "Error while registration user.";

    public async Task<ResultService<(ClaimsPrincipal, string)>> Refresh(string accessToken)
    {
        ResultService<ClaimsPrincipal> verificationResult = await jwtService.VerifyRefreshToken(accessToken);
        if (verificationResult.IsError)
        {
            return ResultService<(ClaimsPrincipal, string)>.Error(verificationResult.ErrorMessage);
        }

        ResultService<string> accessTokenResult = jwtService.GetAccessToken(verificationResult.Result.Claims);
        if (accessTokenResult.IsError)
        {
            ResultService<(ClaimsPrincipal, string)>.Error(accessTokenResult.ErrorMessage);
        }

        return ResultService<(ClaimsPrincipal, string)>.Ok((verificationResult.Result, accessTokenResult.Result));
    }

    public async Task<ResultService<string>> SignIn(string login, string password)
    {
        ResultService<UserEntity> userResult = await GetUser(login, password);
        if (userResult.IsError)
        {
            return ResultService<string>.Error(userResult.ErrorMessage);
        }
        return await AuthenticationProcess(userResult.Result);
    }

    public async Task SignOut()
    {
        string? token = httpContextAccessor.HttpContext?.Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        await jwtService.DisableRefreshToken(token);
        httpContextAccessor.HttpContext?.Response.Cookies.Delete(RefreshTokenCookieName);
    }

    public async Task<ResultService<string>> SignUp(string fullName, string login, string password)
    {
        ResultService<UserEntity> userResult = await GetUser(login, password);
        if (!userResult.IsError)
        {
            return ResultService<string>.Error(WasCreatedError);
        }

        ResultService<UserEntity> registrationResult = await CreateUser(fullName, UserRole.Client, login, password);
        if (registrationResult.IsError)
        {
            return ResultService<string>.Error(registrationResult.ErrorMessage);
        }

        return await AuthenticationProcess(registrationResult.Result);
    }

    private async Task<ResultService<UserEntity>> GetUser(string login, string password)
    {
        UserEntity? dbRecord = await userRepository.GetBy(user => 
            user.Login == login &&
            user.Password == password);

        if (dbRecord is null)
        {
            return ResultService<UserEntity>.Error(InvalidCredentialsError);
        }
        return ResultService<UserEntity>.Ok(dbRecord!);
    }

    private async Task<ResultService<UserEntity>> CreateUser(string fullName, UserRole role, string login, string password)
    {
        UserEntity dbRecord = new()
        {
            FullName = fullName,
            Id = Guid.NewGuid(),
            Login = login,
            Password = password,
            Role = role,
        };

        try
        {
            await userRepository.Add(dbRecord);
        }
        catch
        {
            return ResultService<UserEntity>.Error(RegistrationError);
        }
        return ResultService<UserEntity>.Ok(dbRecord);
    }

    private async Task<ResultService<string>> AuthenticationProcess(UserEntity user)
    {
        IEnumerable<Claim> claims = GetClaims(user);

        ResultService<string> refreshTokenResult = await jwtService.GetRefreshToken(claims);
        if (refreshTokenResult.IsError)
        {
            return ResultService<string>.Error(refreshTokenResult.ErrorMessage);
        }
        httpContextAccessor.HttpContext?.Response.Cookies.Append(RefreshTokenCookieName, refreshTokenResult.Result);

        return jwtService.GetAccessToken(claims);
    }

    private IEnumerable<Claim> GetClaims(UserEntity user) =>
    [
        new(TokenClaimTypes.Id, user.Id.ToString()),
        new(TokenClaimTypes.Name, user.FullName),
        new(TokenClaimTypes.Role, user.Role.ToString())
    ];

    private ClaimsPrincipal GetPrincipal(IEnumerable<Claim> claims) => new(new ClaimsIdentity(claims));

}