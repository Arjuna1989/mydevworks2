using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entity;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        Task CreateUser(User user);
        Task<UserDto> GetUserByIdAsync(int Id);
        Task<User> GetUserByNameAsync(string name);

        Task<UserDto> GetUserDtoByNameAsync(string name);
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<bool> IsUserExist(string name);
        void Update(User user);
        void Delete(User user);
        Task<bool> SaveAll();
    }
}