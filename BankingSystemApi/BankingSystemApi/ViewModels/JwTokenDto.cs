using System.ComponentModel.DataAnnotations;

namespace BankingSystemApi.ViewModels
{
    public class JwTokenDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
