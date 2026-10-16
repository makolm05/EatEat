using Microsoft.EntityFrameworkCore;
using EatEat.Services.CouponAPI.Models;

namespace EatEat.Services.CouponAPI.Data
{
    public class AppDbContext : DbContext
    {
        
        public DbSet<Coupon> Coupons { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData( // we can use UseSeeding as alternative to HasData, but HasData is more straightforward for simple seeding
                new Coupon
                {
                    CouponId = 1,
                    CouponCode = "EATEAT10",
                    DiscountAmount = 10.0,
                    MinAmount = 20,
                    CreatedDate = new DateTime(2026, 1, 1),
                    UpdatedDate = new DateTime(2026, 1, 1)
                },
                new Coupon
                {
                    CouponId = 2,
                    CouponCode = "EATEAT20",
                    DiscountAmount = 20.0,
                    MinAmount = 40,
                    CreatedDate = new DateTime(2026, 1, 1),
                    UpdatedDate = new DateTime(2026, 1, 1)
                }
            );
        }
    }
}
