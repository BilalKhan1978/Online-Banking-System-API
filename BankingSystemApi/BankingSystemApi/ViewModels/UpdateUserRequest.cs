using System.ComponentModel.DataAnnotations;

namespace BankingSystemApi.ViewModels
{
    public class UpdateUserRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
