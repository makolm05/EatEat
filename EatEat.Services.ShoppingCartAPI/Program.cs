using EatEat.Services.ShoppingCartAPI;

var builder = WebApplication.CreateBuilder(args);
builder.RegisterServices();

var app = builder.Build();
app.ConfigurePipeline();

app.Run();