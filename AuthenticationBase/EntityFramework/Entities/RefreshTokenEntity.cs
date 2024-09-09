namespace AuthenticationBase.EntityFramework.Entities;

public sealed record RefreshTokenEntity
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
}