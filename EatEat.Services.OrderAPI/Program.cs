using EatEat.Services.OrderAPI;

var builder = WebApplication.CreateBuilder(args);
builder.RegisterServices();

var app = builder.Build();
app.ConfigurePipeline();

app.Run();