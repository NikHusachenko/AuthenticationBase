using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthenticationBase.Services.JwtServices.Options;

public sealed class TokenValidator
{
    private readonly string _securityKey;

    public TokenValidator(string securityKey)
    {
        _securityKey = securityKey;
    }

    public SymmetricSecurityKey GetSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey));

    public TokenValidationParameters GetValidationParameters() => new()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        NameClaimType = TokenClaimTypes.Name,
        RoleClaimType = TokenClaimTypes.Role,
        IssuerSigningKey = GetSecurityKey(),
    };
}