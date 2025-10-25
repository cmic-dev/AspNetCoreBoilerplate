using AspNetCoreBoilerplate.Shared.Abstractions;
using AspNetCoreBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCoreBoilerplate.Core.Infrastructure.Services;

public class LocalStorageService : IStorageService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<LocalStorageService> _logger;
    private const string FileFolder = "files";

    public LocalStorageService(IWebHostEnvironment webHostEnvironment, ILogger<LocalStorageService> logger)
    {
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(byte[] fileData, string fileName, string contentType, CancellationToken ctn = default)
    {
        if (fileData == null || fileData.Length == 0)
            throw new DomainException("File data cannot be empty");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("File name cannot be empty");

        try
        {
            var wwwrootPath = _webHostEnvironment.WebRootPath;
            var uploadDirectory = Path.Combine(wwwrootPath, FileFolder);

            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadDirectory))
                Directory.CreateDirectory(uploadDirectory);

            // Ensure unique file name
            var uniqueFileName = EnsureUniqueFileName(uploadDirectory, fileName);
            var filePath = Path.Combine(uploadDirectory, uniqueFileName);

            // Save file asynchronously
            await File.WriteAllBytesAsync(filePath, fileData, ctn);

            // Return relative URL
            var relativeUrl = $"/{FileFolder}/{uniqueFileName}";

            _logger.LogInformation("File uploaded successfully: {FileName} at {FilePath}", fileName, relativeUrl);

            return relativeUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            throw new DomainException($"Failed to upload file: {ex.Message}");
        }
    }

    private string EnsureUniqueFileName(string directory, string fileName)
    {
        var filePath = Path.Combine(directory, fileName);

        if (!File.Exists(filePath))
            return fileName;

        var fileExtension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var counter = 1;

        while (File.Exists(Path.Combine(directory, $"{fileNameWithoutExtension}_{counter}{fileExtension}")))
            counter++;

        return $"{fileNameWithoutExtension}_{counter}{fileExtension}";
    }
}