using Microsoft.AspNetCore.Mvc;

namespace AuthenticationBase.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected const string AuthenticationControllerRoute = "api/authentication";
    protected const string SignUpRoute = "sign-up";
    protected const string SignInRoute = "sign-in";

    protected const string UserControllerRoute = "api/user";
    protected const string UserListRoute = "user-list";
    protected const string UserInformationRoute = "information";

    protected async Task<IActionResult> AsError(string message) => BadRequest(new ErrorResponse(message));
    protected async Task<IActionResult> AsSuccess<T>(T result) => Ok(new SuccessResponse<T>(result));
    protected async Task<IActionResult> AsSuccess() => NoContent();
    
    private sealed class ErrorResponse
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public ErrorResponse(string message) => ErrorMessage = message;
    }

    private sealed class SuccessResponse<T>
    {
        public T Result { get; set; }

        public SuccessResponse(T result) => Result = result;
    }
}