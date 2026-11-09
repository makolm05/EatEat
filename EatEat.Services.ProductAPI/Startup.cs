using EatEat.Services.ProductAPI.Data;
using EatEat.Services.ProductAPI.Helpers;
using EatEat.Services.ProductAPI.Helpers.Mapping;
using EatEat.Services.ProductAPI.Services;
using EatEat.Services.ProductAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace EatEat.Services.ProductAPI
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

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.AddSwaggerJWTAuthentication();
            builder.AddJWTAuthentication();

            builder.Services.AddScoped<IProductService, ProductService>();
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
            app.UseStaticFiles();

            app.MapControllers();

            DbHelper.ApplyMigration<AppDbContext>(app);
        }
    }
}