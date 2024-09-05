namespace MeterReading.Database.Services;

public interface ICsvProcessingService
{
    Task<(int successCount, int failureCount)> ProcessMeterReadingsAsync(string meterReadingsCsvPath);
}
