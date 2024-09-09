namespace AuthenticationBase.ApiRequests.Authentication;

public sealed record SignUpApiRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}