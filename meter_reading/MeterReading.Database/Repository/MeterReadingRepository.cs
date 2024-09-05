using MeterReading.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReading.Database.Repository;

public class MeterReadingRepository(ApplicationDbContext context) : IMeterReadingRepository
{
    public async Task<bool> AccountExistsAsync(int accountId)
    {
        return await context.Accounts.AnyAsync(a => a.AccountId == accountId);
    }

    public async Task<bool> IsDuplicateReadingAsync(int accountId, DateTime meterReadDate)
    {
        return await context.MeterReadings.AnyAsync(m =>
            m.AccountId == accountId && m.MeterReadDate == meterReadDate);
    }

    public async Task AddMeterReadingAsync(Models.Reading reading)
    {
        await context.MeterReadings.AddAsync(reading);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
    
}