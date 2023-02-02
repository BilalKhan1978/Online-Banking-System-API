using System.ComponentModel.DataAnnotations;

namespace BankingSystemApi.ViewModels
{
    public class AddUserRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
