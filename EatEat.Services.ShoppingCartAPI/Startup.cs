using System.Text;
using Microsoft.OpenApi;
using EatEat.MessageBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using EatEat.Services.ShoppingCartAPI.Data;
using EatEat.Services.ShoppingCartAPI.Models;
using EatEat.Services.ShoppingCartAPI.Utility;
using EatEat.Services.ShoppingCartAPI.Helpers;
using EatEat.Services.ShoppingCartAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EatEat.Services.ShoppingCartAPI.Helpers.Mapping;
using EatEat.Services.ShoppingCartAPI.Services.IService;

namespace EatEat.Services.ShoppingCartAPI
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
            builder.Services.AddScoped<ICouponService, CouponService>();
            builder.Services.AddHttpClient("Coupon", x => x.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CouponAPI"]))
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