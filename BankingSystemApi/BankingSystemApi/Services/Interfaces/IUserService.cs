using BankingSystemApi.Models;
using BankingSystemApi.ViewModels;

namespace BankingSystemApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetOneUser(int id);
        Task AddUser(AddUserRequest addUserRequest);
        Task UpdateUser(int id, UpdateUserRequest updateUserRequest);
    }
}
