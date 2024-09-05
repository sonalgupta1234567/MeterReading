using MeterReading.Api.Services;
using MeterReading.Database;
using MeterReading.Database.Services;
using Microsoft.EntityFrameworkCore;

namespace MeterReading.Api;

public abstract class StartUp
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<CsvProcessingService>();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
      
    }
    
    public static void Configure(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseForwardedHeaders();
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
      
        app.MapControllers();
        
        app.UseSwagger();
      
    }
}