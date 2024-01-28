using TimeCraft.Core.Helpers;
using TimeCraft.Domain.Dtos.UserDtos;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Core.Services.UserService
{
    public interface IUserService
    {
        Task<User> GetUser(string id);
        Task<List<User>> GetUsers(int page = 1, int pageSize = 10);
        Task<bool> CreateUser(CreateUserDto userDto);
        Task UpdateUser(UpdateUserDto userDto);
        Task DeleteUser(string id);
        Task<AuthResult> RegisterUser(UserRegistration user);
        Task<AuthResult> Login(UserLogin user);
    }
}
