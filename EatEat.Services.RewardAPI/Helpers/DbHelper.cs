using Microsoft.EntityFrameworkCore;

namespace EatEat.Services.RewardsAPI.Helpers
{
    public static class DbHelper
    {
        public static void ApplyMigration<T>(IApplicationBuilder app) where T : DbContext
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<T>();
                if(context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}