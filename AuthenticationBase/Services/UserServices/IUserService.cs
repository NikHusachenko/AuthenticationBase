using AuthenticationBase.EntityFramework.Entities;

namespace AuthenticationBase.Services.UserServices;

public interface IUserService
{
    Task<List<UserEntity>> GetUsers();
}