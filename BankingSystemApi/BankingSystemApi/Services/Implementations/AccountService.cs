using BankingSystemApi.Data;
using BankingSystemApi.Models;
using BankingSystemApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BankingSystemApi.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private BankingDbContext _dbContext; // Injected BankingDbContext class here
        public AccountService(BankingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Account> Authenticate(string AccountNumber, string Pin)
        {
            var account = await _dbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber)
                                             .SingleOrDefaultAsync();
            if (account == null)
                throw new Exception("Account does not exist"); ;
            // if account is not null
            if (!VerifyPinHash(Pin, account.PinHash, account.PinSalt))
                throw new Exception("You have entered incorrect Pin");

            // Authentication verified
            return account;
        }

        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new Exception("Pin");
            // verify pin
            using (var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Pin));
                for (int i = 0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }
            return true;
        }



        public async Task<Account> Create(Account account, string Pin, string ConfirmPin)
        {
            // Create New Account
            if (_dbContext.Accounts.Any(x => x.Email == account.Email)) throw new Exception("An account with this email already exists");

            // Validate Pin
            //if (!Pin.Equals(ConfirmPin)) throw new Exception("Pins do not match");

            // Hashing / encrypting Pin 
            byte[] pinHash, pinSalt;
            CreatePinHash(Pin, out pinHash, out pinSalt);

            account.PinHash = pinHash;
            account.PinSalt = pinSalt;
            account.AccountName = account.FirstName +" "+ account.LastName;
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
            return account;
        }
        // create a method being used in above create method
        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }

        public async Task Delete(int Id)
        {
            var account = _dbContext.Accounts.Find(Id);
            if (account != null)
            {
                _dbContext.Accounts.Remove(account);
                _dbContext.SaveChanges();
            }
        }

        public async Task<IEnumerable<Account>> GetAllAccounts()
        {
            return await _dbContext.Accounts.ToListAsync();
        }

        public async Task<Account> GetByAccountNumber(string AccountNumber)
        {
            var account = await _dbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber)
                                             .FirstOrDefaultAsync();
            if (account == null)
                throw new Exception("Account does not exist");
            return account;
        }

        public async Task<Account> GetById(int Id)
        {
            var account = await _dbContext.Accounts.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (account == null)
                throw new Exception("Account does not exist");
            return account;
        }

        public async Task Update(Account account, string Pin = null)
        {
            var accountUpdated = await _dbContext.Accounts.Where(x => x.Id == account.Id).FirstOrDefaultAsync();
            if (accountUpdated == null) throw new Exception("Account does not exist");


            //user wants to change his/her email
            //checking that new email is available or already taken by someone
            if (!string.IsNullOrWhiteSpace(account.Email))
            { 
                if (_dbContext.Accounts.Any(x => x.Email == account.Email))
                    throw new Exception("This Email" + account.Email + "already exists");
                // change email
                accountUpdated.Email = account.Email;
            }


            // Given option to user to change Email or PhoneNumber or Pin only
            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                //user wants to change his/her PhoneNumber
                //checking that new PhoneNumber is available or already taken by someone
                if (_dbContext.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber))
                    throw new Exception("This PhoneNumber" + account.PhoneNumber + "already exists");
                // change email
                accountUpdated.PhoneNumber = account.PhoneNumber;
            }
            if (!string.IsNullOrWhiteSpace(Pin))
            {
                //user wants to change his/her Pin

                byte[] pinHash, pinSalt;
                CreatePinHash(Pin, out pinHash, out pinSalt);

                accountUpdated.PinHash = pinHash;
                accountUpdated.PinSalt = pinSalt;
            }


            _dbContext.Accounts.Update(accountUpdated);
            await _dbContext.SaveChangesAsync();
        }
    }
}
