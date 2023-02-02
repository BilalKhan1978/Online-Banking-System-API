using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BankingSystemApi.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal CurrentAccountBalance  { get; set; }
        public AccountType AccountType { get; set; }  // Enum
        public string AccountNumberGenerated { get; set; }

        // store hash & salt of the account Transaction pin
        [JsonIgnore]
        public byte[] PinHash { get; set; }
        [JsonIgnore]
        public byte[] PinSalt { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }

        //Create a random obj
        Random rand = new Random();

        //To generate accountnumber in constructor
        public Account()
        {
            //Generate 10 digit random accountNumber for customer
            AccountNumberGenerated = Convert.ToString((long)Math.Floor(rand.NextDouble() * 9_000_000_000L + 1_000_000_000L));

        }
    }
    public enum AccountType
    {
        Savings,
        Current,
        Corporate,
        Government
    }
}
