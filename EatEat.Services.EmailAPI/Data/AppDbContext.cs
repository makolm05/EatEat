using Microsoft.EntityFrameworkCore;
using EatEat.Services.EmailAPI.Models;

namespace EatEat.Services.EmailAPI.Data
{
    public class AppDbContext : DbContext
    {
        
        public DbSet<EmailLog> EmailLogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

    }
}
