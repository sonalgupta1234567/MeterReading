using System.Globalization;
using CsvHelper;
using MeterReading.Database.Models;
using MeterReading.Database.Repository;
using MeterReading.Database.Services;

namespace MeterReading.Api.Services
{
    public class CsvProcessingService(IMeterReadingRepository repository) : ICsvProcessingService
    {
        public async Task<(int successCount, int failureCount)> ProcessMeterReadingsAsync(string meterReadingsCsvPath)
        {
            var successCount = 0;
            var failureCount = 0;
            
            using (var reader = new StreamReader(meterReadingsCsvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

                 csv.Context.RegisterClassMap<ReadingMap>();  // Register the map
                var meterReadings = csv.GetRecords<Reading>().ToList();

                foreach (var item in meterReadings)
                {
                    if (await ValidateMeterReadingAsync(item))
                    {
                        await repository.AddMeterReadingAsync(item);  // Call the repository to add the reading
                     
                        successCount++;
                    }
                    else
                    {
                        failureCount++;
                    }
                }
                await repository.SaveChangesAsync();  // Call the repository to save changes
            }

            return (successCount, failureCount);
        }

        private async Task<bool> ValidateMeterReadingAsync(Database.Models.Reading reading)
        {
            // Check if Account exists
            if (!await repository.AccountExistsAsync(reading.AccountId)) return false; // Invalid AccountId

            // Check if Meter Value is a 5-digit number
            if (reading.MeterValue.ToString().Length != 5) return false; // Invalid meter value format

            // Check for duplicate entries
            return !await repository.IsDuplicateReadingAsync(reading.AccountId, reading.MeterReadDate);
        }
    }
}
