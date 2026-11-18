using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using EatEat.GatewaySolution.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddJWTAuthentication();
if (builder.Environment.EnvironmentName.ToString().ToLower().Equals("production"))
{
    builder.Configuration.AddJsonFile("ocelot.Production.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
}
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();
await app.UseOcelot();

app.Run();
