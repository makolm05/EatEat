using Microsoft.EntityFrameworkCore;
using EatEat.Services.EmailAPI.Data;
using EatEat.Services.EmailAPI.Helpers;
using EatEat.Services.EmailAPI.Messaging.Interfaces;
using EatEat.Services.EmailAPI.Messaging;
using EatEat.Services.EmailAPI.Services;

namespace EatEat.Services.EmailAPI
{
    public static class Startup
    {
        private static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }

        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var optBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            builder.Services.AddSingleton(new EmailService(optBuilder.Options));

            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddAuthorization();
        }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostAppliactionLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostAppliactionLife.ApplicationStarted.Register(OnStart);
            hostAppliactionLife.ApplicationStopping.Register(OnStop);
            return app;
        }

        private static void OnStart()
        {
            ServiceBusConsumer.Start();
        }

        private static void OnStop()
        {
            ServiceBusConsumer.Stop();
        }

        public static void ConfigurePipeline(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "EatEat.Services.EmailAPI v1");
                x.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            DbHelper.ApplyMigration<AppDbContext>(app);

            app.UseAzureServiceBusConsumer();
        }
    }
}