using System.Globalization;
using CsvHelper;

namespace MeterReading.Database;

public static class DbSeeder
{
    public static void SeedAccounts(ApplicationDbContext context, string accountsCsvPath)
    {
        if (!context.Accounts.Any())
        {
            using var reader = new StreamReader(accountsCsvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var accounts = csv.GetRecords<Models.Account>().ToList();
            context.Accounts.AddRange((IEnumerable<Models.Account>)accounts);
            context.SaveChanges();
        }
    }
}