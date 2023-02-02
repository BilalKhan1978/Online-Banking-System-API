using AutoMapper;
using BankingSystemApi.Models;
using BankingSystemApi.Services.Interfaces;
using BankingSystemApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BankingSystemApi.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController : Controller
    {
        private IAccountService _accountService;
        IMapper _mapper;

        public AccountsController(IAccountService accountService, IMapper mapper)
        {
            this._accountService = accountService;
            this._mapper = mapper;  
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            { 
            var accounts = await _accountService.GetAllAccounts();
            var cleanedAccounts = _mapper.Map<IList<GetAccountModel>>(accounts);
            return Ok(cleanedAccounts);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> NewAccount([FromBody] RegisterNewAccountModel newAccount)
        {
           try
           { 
              var account = _mapper.Map<Account>(newAccount);
              return Ok(await _accountService.Create(account, newAccount.Pin, newAccount.ConfirmPin));
           }
           catch(Exception e)
           {
                if (e.Message.Contains("An account with this email already exists"))
                    return Conflict(new { message = "An account with this email already exists"});
                throw new Exception(e.Message);
           }
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
        {
          try
          { 
             return Ok(await _accountService.Authenticate(model.AccountNumber, model.Pin));
          }
          catch(Exception e)
          {
                throw new Exception(e.Message);
          }
        }

        [HttpGet("GetByAccountNumber")]
        public async Task<IActionResult> GetByAccountNumber(string AccountNumber)
        {
            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                return BadRequest("Account Number must be 10-digit");
            try
            {
                var account = await _accountService.GetByAccountNumber(AccountNumber);
                var cleanedAccount = _mapper.Map<GetAccountModel>(account);
                return Ok(cleanedAccount);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Account does not exist"))
                    return NotFound(e.Message);
                throw new Exception(e.Message);
            }
        }

        [HttpGet("GetAccountById")]
        public async Task<IActionResult> GetAccountById(int Id)
        {
            try
            { 
            var account = await _accountService.GetById(Id);  
            var cleanedAccount = _mapper.Map<GetAccountModel>(account);
            return Ok(cleanedAccount);
            }
            catch(Exception e)
            {
                if (e.Message.Contains("Account does not exist"))
                    return NotFound(e.Message);
                throw new Exception(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountModel model)
        {
            try
            { 
            var account = _mapper.Map<Account>(model);
            await _accountService.Update(account, model.Pin);
            return Ok();
            }
            catch(Exception e)
            {
                if (e.Message.Contains("Account does not exist"))
                    return NotFound(e.Message);
                throw new Exception(e.Message);
            }
        }
    }
}
