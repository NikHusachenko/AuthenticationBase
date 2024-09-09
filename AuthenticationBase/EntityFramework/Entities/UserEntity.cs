using AuthenticationBase.EntityFramework.Enums;

namespace AuthenticationBase.EntityFramework.Entities;

public sealed record UserEntity
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public List<RefreshTokenEntity> RefreshTokens { get; set; } = [];
}