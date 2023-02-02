using BankingSystemApi.Models;

namespace BankingSystemApi.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account> Authenticate(string AccountNumber, string Pin); // return type Account class
        Task<IEnumerable<Account>> GetAllAccounts();
        Task<Account> Create(Account account, string Pin, string ConfirmPin);
        Task Update(Account account, string Pin = null);
        Task Delete(int Id);
        Task<Account> GetById(int Id);
        Task<Account> GetByAccountNumber(string AccountNumber);
    }
}
