using MeterReading.Database.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeterReading.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeterReadingController(ICsvProcessingService csvService) : ControllerBase
{
    [HttpPost($"meter-reading-uploads")]
    public async Task<IActionResult> UploadMeterReadings(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File not provided.");
        }

        // Save the uploaded file locally for processing
        var filePath = Path.GetTempFileName(); // Generate a temp file path
        await using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream); // Asynchronously copy the file
        }

        // Asynchronously process the CSV file
        var (successCount, failureCount) = await csvService.ProcessMeterReadingsAsync(filePath);

        // Return the result of processing
        return Ok(new SuccessFailureResult
        {
            SuccessCount = successCount,
            FailureCount = failureCount
        });
    }
}

