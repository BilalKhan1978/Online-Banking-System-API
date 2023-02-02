using BankingSystemApi.Data;
using BankingSystemApi.Models;
using BankingSystemApi.Services.Interfaces;
using BankingSystemApi.ViewModels;

namespace BankingSystemApi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly BankingDbContext _dbContext;

        public UserService(BankingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetOneUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            return user;
        }
        public async Task AddUser(AddUserRequest addUserRequest)
        {
            var user = new User()
            {
                Name = addUserRequest.Name,
                Email = addUserRequest.Email,
                Password = addUserRequest.Password
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateUser(int id, UpdateUserRequest updateUserRequest)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("Not found");
            }
            user.Name = updateUserRequest.Name;
            user.Email = updateUserRequest.Email;
            user.Password = updateUserRequest.Password;
            await _dbContext.SaveChangesAsync();
        }
    }
}
