using FluentAssertions;
using NSubstitute;
using MeterReading.Database.Models;
using MeterReading.Api.Services;
using MeterReading.Database.Repository;

namespace MeterReading.Tests
{
    [TestFixture]
    public class CsvProcessingServiceTests
    {
        private IMeterReadingRepository _mockRepository;
        private CsvProcessingService _csvProcessingService;

        [SetUp]
        public void Setup()
        {
            // Initialize the mock repository using NSubstitute
            _mockRepository = Substitute.For<IMeterReadingRepository>();
            _csvProcessingService = new CsvProcessingService(_mockRepository);
        }

        [Test]
        public async Task ProcessMeterReadingsAsync_ValidReadings_SuccessfulProcessing()
        {
            // Arrange: Mock repository to simulate a valid account and no duplicate readings
            _mockRepository.AccountExistsAsync(1).Returns(true);
            _mockRepository.IsDuplicateReadingAsync(1, Arg.Any<DateTime>()).Returns(false);

            // Mock CSV content with valid data
            var csvContent = "AccountId,MeterReadDate,MeterValue\n1,2023-01-01,12345";
            var meterReadingsCsvPath = WriteCsvToTempFile(csvContent);


            // Act: Process the CSV
            var (successCount, failureCount) = await _csvProcessingService.ProcessMeterReadingsAsync(meterReadingsCsvPath);

            // Assert: Verify the success count and repository interactions
            successCount.Should().Be(1);
            failureCount.Should().Be(0);
            await _mockRepository.Received(1).AddMeterReadingAsync(Arg.Any<Reading>());
            await _mockRepository.Received(1).SaveChangesAsync();
        }

        [Test]
        public async Task ProcessMeterReadingsAsync_InvalidAccount_FailureProcessing()
        {
           // Arrange: Mock repository to simulate a missing account
            _mockRepository.AccountExistsAsync(2).Returns(false);

         //   Mock CSV content with an invalid account
            var csvContent = "AccountId,MeterReadDate,MeterValue\n2,2023-01-01,12345";
            var meterReadingsCsvPath = WriteCsvToTempFile(csvContent);

         //   Act: Process the CSV
            var (successCount, failureCount) = await _csvProcessingService.ProcessMeterReadingsAsync(meterReadingsCsvPath);

           // Assert: Verify failure processing and no changes saved
            successCount.Should().Be(0);
            failureCount.Should().Be(1);
            await _mockRepository.DidNotReceive().AddMeterReadingAsync(Arg.Any<Reading>());
            await _mockRepository.Received(1).SaveChangesAsync();
        }

        [Test]
        public async Task ProcessMeterReadingsAsync_InvalidMeterValue_FailureProcessing()
        {
            // Arrange: Mock repository to simulate a valid account
            _mockRepository.AccountExistsAsync(1).Returns(true);

            // Mock CSV content with an invalid meter value (less than 5 digits)
            var csvContent = "AccountId,MeterReadDate,MeterValue\n1,2023-01-01,1234";
            var meterReadingsCsvPath = WriteCsvToTempFile(csvContent);

            // Act: Process the CSV
            var (successCount, failureCount) = await _csvProcessingService.ProcessMeterReadingsAsync(meterReadingsCsvPath);

            // Assert: Verify failure processing and no changes saved
            successCount.Should().Be(0);
            failureCount.Should().Be(1);
            await _mockRepository.DidNotReceive().AddMeterReadingAsync(Arg.Any<Reading>());
            await _mockRepository.Received(1).SaveChangesAsync();
        }

        [Test]
        public async Task ProcessMeterReadingsAsync_DuplicateReading_FailureProcessing()
        {
            // Arrange: Mock repository to simulate valid account and duplicate reading
            _mockRepository.AccountExistsAsync(1).Returns(true);
            _mockRepository.IsDuplicateReadingAsync(1, Arg.Any<DateTime>()).Returns(true);

            // Mock CSV content with a duplicate reading
            const string csvContent = "AccountId,MeterReadDate,MeterValue\n1,2023-01-01,125";
            var meterReadingsCsvPath = WriteCsvToTempFile(csvContent);

            // Act: Process the CSV
            var (successCount, failureCount) = await _csvProcessingService.ProcessMeterReadingsAsync(meterReadingsCsvPath);

            // Assert: Verify failure processing and no changes saved
            successCount.Should().Be(0);
            failureCount.Should().Be(1);
            await _mockRepository.DidNotReceive().AddMeterReadingAsync(Arg.Any<Reading>());
            await _mockRepository.Received(1).SaveChangesAsync();
        }

        // Helper method to write CSV content to a temporary file
        private static string WriteCsvToTempFile(string csvContent)
        {
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, csvContent);
            return tempFilePath;
        }
    }
}
