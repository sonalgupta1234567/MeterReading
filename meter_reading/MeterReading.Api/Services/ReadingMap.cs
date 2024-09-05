using MeterReading.Database.Models;
using CsvHelper.Configuration;

namespace MeterReading.Api.Services
{
    public sealed class ReadingMap : ClassMap<Reading>
    {
        public ReadingMap()
        {
            Map(m => m.AccountId).Name("AccountId");
            Map(m => m.MeterReadDate).Name("MeterReadDate");
            Map(m => m.MeterValue).Name("MeterValue");
        }
    }
}