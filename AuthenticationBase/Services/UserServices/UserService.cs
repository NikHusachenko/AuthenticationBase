using AuthenticationBase.EntityFramework.Entities;
using AuthenticationBase.EntityFramework.Repository;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationBase.Services.UserServices;

public sealed class UserService(IGenericRepository<UserEntity> repository) : IUserService
{
    public Task<List<UserEntity>> GetUsers() => repository.GetAll().ToListAsync();
}