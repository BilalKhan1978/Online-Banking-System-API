using BankingSystemApi.Models;
using System.ComponentModel.DataAnnotations;

namespace BankingSystemApi.ViewModels
{
    public class TransactionRequestDto
    {
        public decimal TransactionAmount { get; set; }
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public TranType TransactionType { get; set; } //enum
        public DateTime TransactionDate { get; set; }
    }
}
