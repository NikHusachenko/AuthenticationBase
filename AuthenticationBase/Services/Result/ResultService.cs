namespace AuthenticationBase.Services.Result;

public sealed class ResultService
{
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public static ResultService Ok() => new();
    public static ResultService Error(string message) =>
        new()
        {
            ErrorMessage = message,
            IsError = true
        };
}

public sealed class ResultService<T>
{
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public T Result { get; set; }

    public static ResultService<T> Ok(T result) => new() { Result = result };
    public static ResultService<T> Error(string message) =>
        new()
        {
            ErrorMessage = message,
            IsError = true
        };
}