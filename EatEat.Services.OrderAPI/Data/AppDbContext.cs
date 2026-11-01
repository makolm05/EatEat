using Microsoft.EntityFrameworkCore;
using EatEat.Services.OrderAPI.Models;

namespace EatEat.Services.OrderAPI.Data
{
    public class AppDbContext : DbContext
    {
        
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

    }
}
