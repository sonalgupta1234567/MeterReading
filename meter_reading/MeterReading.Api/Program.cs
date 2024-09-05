using MeterReading.Api;

var builder = WebApplication.CreateBuilder(args);

StartUp.ConfigureServices(builder.Services, builder.Configuration);
var app = builder.Build();

StartUp.Configure(app);

app.Run();
