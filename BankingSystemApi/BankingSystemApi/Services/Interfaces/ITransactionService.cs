using BankingSystemApi.Models;

namespace BankingSystemApi.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Response> CreateNewTransaction(Transaction transaction); // return type response class
        Task<Response> FindTransactionByDate(DateTime date);
        Task<Response> MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin);
        Task<Response> MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin);
        Task<Response> MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin);
    }
}
