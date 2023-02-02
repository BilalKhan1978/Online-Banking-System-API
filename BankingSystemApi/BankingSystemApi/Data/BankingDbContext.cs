using BankingSystemApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemApi.Data
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(DbContextOptions options) : base(options)
        {
        }
      public DbSet<Account> Accounts { get; set; }
      public DbSet<Transaction> Transactions { get; set; }
      public DbSet<User> Users { get; set; }   // This table is for JwToken only
    }
}
