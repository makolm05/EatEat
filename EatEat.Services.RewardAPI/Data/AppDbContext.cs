using Microsoft.EntityFrameworkCore;
using EatEat.Services.RewardAPI.Models;

namespace EatEat.Services.RewardsAPI.Data
{
    public class AppDbContext : DbContext
    {
        
        public DbSet<Rewards> Rewards { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

    }
}
