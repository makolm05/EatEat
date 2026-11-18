using Stripe;
using System.Text;
using Microsoft.OpenApi;
using EatEat.MessageBus;
using EatEat.Services.OrderAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using EatEat.Services.OrderAPI.Utility;
using EatEat.Services.OrderAPI.Helpers;
using EatEat.Services.OrderAPI.Helpers.Mapping;
using EatEat.Services.OrderAPI.Services.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProductService = EatEat.Services.OrderAPI.Services.ProductService; // because Stripe.ProductService is also a class, so we need to use alias to avoid ambiguity

namespace EatEat.Services.OrderAPI
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

            builder.Services.AddOptions<AzureServiceBusSettings>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    options.AzureServiceBusConnectionString =
                        configuration.GetConnectionString("AzureServiceBusConnectionString")
                        ?? string.Empty;
                });
            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<BackendAPIAuthenticationHttpClientHandler>();
            builder.Services.AddScoped<IMessageBus, AzureMessageBus>();

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddHttpClient("Product", x => x.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]))
                .AddHttpMessageHandler<BackendAPIAuthenticationHttpClientHandler>();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.AddSwaggerJWTAuthentication();

            builder.AddJWTAuthentication();
            builder.Services.AddAuthorization();
        }

        public static void AddSwaggerJWTAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = $"Add the Bearer Authorizations string as followng: '{JwtBearerDefaults.AuthenticationScheme} {{token}}'",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                opt.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                    JwtBearerDefaults.AuthenticationScheme, document)] = []
                });
            });
        }

        public static void AddJWTAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("ApiSettings:Secret"))),

                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration.GetValue<string>("ApiSettings:Issuer"),

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration.GetValue<string>("ApiSettings:Audience"),
                };
            });
        }

        public static void ConfigurePipeline(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "EatEat.Services.OrderAPI v1");
                x.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            DbHelper.ApplyMigration<AppDbContext>(app);

            StripeConfiguration.ApiKey = app.Configuration.GetValue<string>("Stripe:SecretKey");
        }
    }
}