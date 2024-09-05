using MeterReading.Database.Models;

namespace MeterReading.Database.Repository;

public interface IMeterReadingRepository
{
    Task<bool> AccountExistsAsync(int accountId);
    Task<bool> IsDuplicateReadingAsync(int accountId, DateTime meterReadDate);
    Task SaveChangesAsync();
    Task AddMeterReadingAsync(Reading reading);
}