using AuthenticationBase.EntityFramework.Entities;
using AuthenticationBase.EntityFramework.Repository;
using AuthenticationBase.Services.JwtServices.Options;
using AuthenticationBase.Services.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthenticationBase.Services.JwtServices;

public sealed class JwtService : IJwtService
{
    private const string CanReadTokenError = "Invalid token.";
    private const string CreateTokenError = "Error while create token.";
    private const string DisablingOperationError = "Error while disabling token";
    private const string UnauthorizeError = "Unauthorize";

    private readonly IGenericRepository<RefreshTokenEntity> _repository;
    private readonly JwtOptions _options;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TokenValidator _tokenValidator;

    public JwtService(IGenericRepository<RefreshTokenEntity> repository,
        IOptionsMonitor<JwtOptions> optionsMonitor)
    {
        _repository = repository;
        _options = optionsMonitor.CurrentValue;
        _tokenHandler = new JwtSecurityTokenHandler();
        _tokenValidator = new TokenValidator(_options.Key);
    }

    public ResultService<IEnumerable<Claim>> Decode(string token)
    {
        if (string.IsNullOrEmpty(token) || !_tokenHandler.CanReadToken(token))
        {
            return ResultService<IEnumerable<Claim>>.Error(CanReadTokenError);
        }

        JwtSecurityToken securityToken = _tokenHandler.ReadJwtToken(token);
        return ResultService<IEnumerable<Claim>>.Ok(securityToken.Claims);
    }

    public async Task<ResultService> DisableRefreshToken(string token)
    {
        RefreshTokenEntity? dbRecord = await _repository.GetBy(t => t.Token == token);
        if (dbRecord is null)
        {
            return ResultService.Error(UnauthorizeError);
        }

        dbRecord.IsActive = false;

        try
        {
            await _repository.Update(dbRecord);
        }
        catch
        {
            return ResultService.Error(DisablingOperationError);
        }
        return ResultService.Ok();
    }

    public ResultService<string> GetAccessToken(IEnumerable<Claim> claims) => GetToken(claims, _options.AccessExpiration);

    public async Task<ResultService<string>> GetRefreshToken(IEnumerable<Claim> claims)
    {
        ResultService<string> tokenResult = GetToken(claims, _options.RefreshExpiration);
        if (tokenResult.IsError)
        {
            return tokenResult;
        }

        ResultService<Guid> uidResult = ExtractUserId(claims);
        if (uidResult.IsError)
        {
            return ResultService<string>.Error(uidResult.ErrorMessage);
        }

        RefreshTokenEntity dbRecord = new()
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            Token = tokenResult.Result,
            UserId = uidResult.Result
        };

        try
        {
            await _repository.Add(dbRecord);
        }
        catch
        {
            return ResultService<string>.Error(CreateTokenError);
        }
        return ResultService<string>.Ok(dbRecord.Token);
    }

    public async Task<ResultService<ClaimsPrincipal>> VerifyRefreshToken(string token)
    {
        RefreshTokenEntity? dbRecord = await _repository.GetAll()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);

        if (dbRecord is null)
        {
            return ResultService<ClaimsPrincipal>.Error(UnauthorizeError);
        }
        
        if (!dbRecord.IsActive)
        {
            return ResultService<ClaimsPrincipal>.Error(UnauthorizeError);
        }

        try
        {
            return ResultService<ClaimsPrincipal>.Ok(_tokenHandler.ValidateToken(token,
                _tokenValidator.GetValidationParameters(),
                out SecurityToken securityToken));
        }
        catch
        {
            return ResultService<ClaimsPrincipal>.Error(UnauthorizeError);
        }
    }

    private ResultService<string> GetToken(IEnumerable<Claim> claims, int expiration)
    {
        SymmetricSecurityKey securityKey = _tokenValidator.GetSecurityKey();
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken securityToken = new(claims: claims,
            signingCredentials: credentials,
            expires: DateTime.Now.AddSeconds(expiration));

        try
        {
            return ResultService<string>.Ok(_tokenHandler.WriteToken(securityToken));
        }
        catch (Exception ex)
        {
            return ResultService<string>.Error(CreateTokenError);
        }
    }

    private ResultService<Guid> ExtractUserId(IEnumerable<Claim> claims)
    {
        Claim? claim = claims.FirstOrDefault(claim => claim.Type == TokenClaimTypes.Id);
        if (claim is null)
        {
            return ResultService<Guid>.Error(CreateTokenError);
        }

        if (!Guid.TryParse(claim.Value, out Guid id))
        {
            return ResultService<Guid>.Error(CreateTokenError);
        }
        
        return ResultService<Guid>.Ok(id);
    }
}