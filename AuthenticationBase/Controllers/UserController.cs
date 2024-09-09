using AuthenticationBase.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationBase.Controllers;

[Route(UserControllerRoute)]
public class UserController(IUserService userService) : BaseController
{
    [HttpGet(UserListRoute)]
    public async Task<IActionResult> GetUsers() => await AsSuccess(await userService.GetUsers());
}