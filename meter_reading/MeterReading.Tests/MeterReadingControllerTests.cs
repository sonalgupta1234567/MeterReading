using FluentAssertions;
using MeterReading.Api.Controllers;
using MeterReading.Database.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace MeterReading.Tests;

[TestFixture]
public class MeterReadingControllerTests
{
    private MeterReadingController _controller;
    private ICsvProcessingService _csvProcessingService;

    [SetUp]
    public void Setup()
    {
        // Substitute the service
        _csvProcessingService = Substitute.For<ICsvProcessingService>();
        _controller = new MeterReadingController(_csvProcessingService);
    }

    [Test]
    public async Task UploadMeterReadings_ValidFile_ReturnsSuccessResult()
    {
        // Arrange
        var formFile = Substitute.For<IFormFile>();
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream);
        
        await writer.WriteAsync("AccountId,MeterReadDate,MeterValue\n1,2023-01-01,12345");
        await writer.FlushAsync();
        memoryStream.Position = 0;

        formFile.OpenReadStream().Returns(memoryStream);
        formFile.Length.Returns(memoryStream.Length);

        // Mock the CsvProcessingService behavior
        _csvProcessingService.ProcessMeterReadingsAsync(Arg.Any<string>())
            .Returns(Task.FromResult((successCount: 1, failureCount: 0)));

        // Act
        var result = await _controller.UploadMeterReadings(formFile);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        var resultData = okResult?.Value as SuccessFailureResult;
         resultData?.SuccessCount.Should().Be(1);
         resultData?.FailureCount.Should().Be(0);
    }

    [Test]
    public async Task UploadMeterReadings_FileNotProvided_ReturnsBadRequest()
    {
        // Arrange
        IFormFile? file = null;

        // Act
        var result = await _controller.UploadMeterReadings(file);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult?.Value.Should().Be("File not provided.");
    }

    [Test]
    public async Task UploadMeterReadings_ServiceFails_ReturnsFailureCount()
    {
        // Arrange
        var formFile = Substitute.For<IFormFile>();
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream);
        await writer.WriteAsync("AccountId,MeterReadDate,MeterValue\n1,2023-01-01,12345");
        await writer.FlushAsync();
        memoryStream.Position = 0;

        formFile.OpenReadStream().Returns(memoryStream);
        formFile.Length.Returns(memoryStream.Length);

        // Mock service to simulate failures
        _csvProcessingService.ProcessMeterReadingsAsync(Arg.Any<string>())
            .Returns(Task.FromResult((successCount: 0, failureCount: 1)));

        // Act
        var result = await _controller.UploadMeterReadings(formFile);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        var resultData = okResult?.Value as SuccessFailureResult;
        resultData.SuccessCount.Should().Be(0);
        resultData.FailureCount.Should().Be(1);
    }
}