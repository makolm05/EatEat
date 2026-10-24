using Microsoft.EntityFrameworkCore;
using EatEat.Services.ShoppingCartAPI.Models;

namespace EatEat.Services.ShoppingCartAPI.Data
{
    public class AppDbContext : DbContext
    {
        
        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

    }
}
