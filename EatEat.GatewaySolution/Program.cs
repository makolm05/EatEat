using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using EatEat.GatewaySolution.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddJWTAuthentication();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();
await app.UseOcelot();

app.Run();
