using AutoMapper;
using BankingSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BankingSystemApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : Controller
    {
        private ITransactionService _transactionService;  // Inject the TransactionService class
        IMapper _mapper;

        public TransactionsController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        //[HttpPost("create_new_transaction")]
        //public async Task<IActionResult> CreateNewTransaction([FromBody] TransactionRequestDto transactionRequest)
        //{
        //    var transaction = _mapper.Map<Transaction>(transactionRequest);
        //    return Ok(await _transactionService.CreateNewTransaction(transaction));
        //}

        [HttpPost("make_deposit")]
        public async Task<IActionResult> MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                return BadRequest("Account Number must be 10-digit");
            return Ok(await _transactionService.MakeDeposit(AccountNumber, Amount, TransactionPin));
        }
        
        [HttpPost("make_withdrawal")]
        public async Task<IActionResult> MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                return BadRequest("Account Number must be 10-digit");
            return Ok(await _transactionService.MakeWithdrawal(AccountNumber, Amount, TransactionPin));
        }

        [HttpPost("make_funds_transfer")]
        public async Task<IActionResult> MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            if (!Regex.IsMatch(FromAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$") || !Regex.IsMatch(ToAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                return BadRequest("Account Number must be 10-digit");
            return Ok(await _transactionService.MakeFundsTransfer(FromAccount, ToAccount, Amount, TransactionPin));
        }
    }
}
