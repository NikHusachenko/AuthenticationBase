namespace AuthenticationBase.Services.JwtServices.Options;

public sealed record JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public int AccessExpiration { get; set; }
    public int RefreshExpiration { get; set; }
}