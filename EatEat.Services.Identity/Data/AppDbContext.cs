using Microsoft.EntityFrameworkCore;
using EatEat.Services.IdentityAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EatEat.Services.IdentityAPI.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}