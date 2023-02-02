using AutoMapper;
using BankingSystemApi.Models;
using BankingSystemApi.ViewModels;

namespace BankingSystemApi.Mapper
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<RegisterNewAccountModel, Account>();

            CreateMap<UpdateAccountModel, Account>();

            CreateMap<Account, GetAccountModel>();

            CreateMap<TransactionRequestDto, Transaction>();
        }
    }
}
