using Microsoft.EntityFrameworkCore;
using EatEat.Services.EmailAPI.Data;
using EatEat.Services.EmailAPI.Helpers;

namespace EatEat.Services.EmailAPI
{
    public static class Startup
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddAuthorization();
        }

        public static void ConfigurePipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            DbHelper.ApplyMigration<AppDbContext>(app);
        }
    }
}