using Microsoft.EntityFrameworkCore;

namespace MeterReading.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Models.Account> Accounts { get; set; }
    public DbSet<Models.Reading> MeterReadings { get; set; }
}
