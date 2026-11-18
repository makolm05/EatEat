using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EatEat.Services.IdentityAPI.Data;
using EatEat.Services.IdentityAPI.Models;
using EatEat.Services.IdentityAPI.Helpers;
using EatEat.Services.IdentityAPI.Services;
using EatEat.Services.CouponAPI.Helpers.Mapping;
using EatEat.Services.IdentityAPI.Services.IServices;
using EatEat.MessageBus;

namespace EatEat.Services.IdentityAPI
{
    public static class Startup
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddAutoMapper(cfg => {
                cfg.AddProfile<MappingProfile>();
            });

            builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("APISettings:JWTOptions"));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            
            builder.Services.AddControllers();

            builder.Services.AddOptions<AzureServiceBusSettings>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                options.AzureServiceBusConnectionString =
                    configuration.GetConnectionString("AzureServiceBusConnectionString")
                    ?? string.Empty;
            });

            builder.Services.AddScoped<IMessageBus, AzureMessageBus>();

            builder.Services.AddScoped<IJWTTokenGenerator, JWTTokenGenerator>();
            builder.Services.AddScoped<IIdentityService, IdentityService>();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
        }

        public static void ConfigurePipeline(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "EatEat.Services.IdentityAPI v1");
                x.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            DbHelper.ApplyMigration<AppDbContext>(app);
        }
    }
}