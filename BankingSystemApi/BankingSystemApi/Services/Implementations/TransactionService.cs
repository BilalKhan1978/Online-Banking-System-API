using BankingSystemApi.Data;
using BankingSystemApi.Models;
using BankingSystemApi.Services.Interfaces;
using BankingSystemApi.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BankingSystemApi.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private BankingDbContext _dbContext;
        
        ILogger<TransactionService> _logger;

        private AppSettings _settings;

        private static string _ourBankSettlementAccount;

        private readonly IAccountService _accountService;

        public TransactionService(BankingDbContext dbContext, ILogger<TransactionService> logger,
                                  IOptions<AppSettings> settings, IAccountService accountService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _settings = settings.Value;
            _ourBankSettlementAccount = _settings.BankSettlementAccount;
            _accountService = accountService;
        }

        public async Task<Response> CreateNewTransaction(Transaction transaction)
        {
            Response response = new Response();

            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction has been made successfully";
            response.Data = null;
            return response;    
        }

        public async Task<Response> FindTransactionByDate(DateTime date)
        {
            Response response = new Response();
            var transaction = await _dbContext.Transactions.Where(x => x.TransactionDate == date).ToListAsync();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction has been made successfully";
            response.Data = null;
            return response;
        }

        public async Task<Response> MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            // Checking user - account owner is valid

            var authUser = await _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null) throw new Exception("Invalid credentials");

            // If validation is ok then

            try
            {
                // For deposit, bankSettlementAccount is the source giving money to user account
                sourceAccount = await _accountService.GetByAccountNumber(_ourBankSettlementAccount);
                destinationAccount = await _accountService.GetByAccountNumber(AccountNumber);
            
                // Update account balance
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;  

                // check these updates

                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successful";
                    response.Data = null;
                    
                }
                else
                {
                    // transaction is unsuccessful
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction Failed";
                    response.Data = null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An Error Occurred... { e.Message }");
            }

            //set transaction properties

            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionType = TranType.Deposit;
            transaction.TransactionAmount = Amount;
            transaction.TransactionSourceAccount = _ourBankSettlementAccount;
            transaction.TransactionDestinationAccount = AccountNumber;
            transaction.TransactionParticulars = $"NEW Transaction FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT =>  {JsonConvert.SerializeObject (transaction.TransactionAmount)} TRANSACTION TYPE => {JsonConvert.SerializeObject(transaction.TransactionType)} TRANSACTION STATUS => {JsonConvert.SerializeObject(transaction.TransactionStatus)}";


            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();


            return response;


        }

        public async Task<Response> MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            // Checking user - account owner is valid

            var authUser = await _accountService.Authenticate(FromAccount, TransactionPin);
            if (authUser == null) throw new Exception("Invalid credentials");

            // If validation is ok then

            try
            {
                // For deposit, bankSettlementAccount is the destination getting money from to user account
                sourceAccount = await _accountService.GetByAccountNumber(FromAccount);
                destinationAccount = await _accountService.GetByAccountNumber(ToAccount);

                // Update account balance
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // check these updates

                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // transaction is successfull
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successful";
                    response.Data = null;

                }
                else
                {
                    // transaction is unsuccessfull
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction Failed";
                    response.Data = null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An Error Occurred... {e.Message}");
            }

            //set transaction properties

            
            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = FromAccount;
            transaction.TransactionDestinationAccount = ToAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            
            transaction.TransactionParticulars = $"NEW Transaction FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT =>  {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => {JsonConvert.SerializeObject(transaction.TransactionType)} TRANSACTION STATUS => {JsonConvert.SerializeObject(transaction.TransactionStatus)}";

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();


            return response;
        }

        public async Task<Response> MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            // Checking user - account owner is valid

            var authUser = await _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null) throw new Exception("Invalid credentials");

            // If validation is ok then

            try
            {
                // For deposit, bankSettlementAccount is the destination getting money from to user account
                sourceAccount = await _accountService.GetByAccountNumber(AccountNumber);
                destinationAccount = await _accountService.GetByAccountNumber(_ourBankSettlementAccount);

                // Update account balance
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // check these updates

                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successfull";
                    response.Data = null;

                }
                else
                {
                    // transaction is unsuccessful
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction Failed";
                    response.Data = null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An Error Occurred... {e.Message}");
            }

            //set transaction properties

            transaction.TransactionType = TranType.Withdrawal;
            transaction.TransactionSourceAccount = AccountNumber; 
            transaction.TransactionDestinationAccount = _ourBankSettlementAccount; 
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            
            transaction.TransactionParticulars = $"NEW Transaction FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT =>  {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => {JsonConvert.SerializeObject(transaction.TransactionType)} TRANSACTION STATUS => {JsonConvert.SerializeObject(transaction.TransactionStatus)}";

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();


            return response;
        }
    }
}
