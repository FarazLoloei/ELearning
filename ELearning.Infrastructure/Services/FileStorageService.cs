using ELearning.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ELearning.Infrastructure.Services;

public class FileStorageService(ILogger<FileStorageService> logger) : IFileStorageService
{
    public Task<string> SaveFileAsync(byte[] content, string fileName, string contentType)
    {
        logger.LogInformation("File {FileName} of type {ContentType} saved", fileName, contentType);
        // In a real application, implement actual file storage logic
        return Task.FromResult($"https://example.com/files/{Guid.NewGuid()}/{fileName}");
    }

    public Task<byte[]> GetFileAsync(string fileUrl)
    {
        logger.LogInformation("File {FileUrl} retrieved", fileUrl);
        // In a real application, implement actual file retrieval logic
        return Task.FromResult(new byte[0]);
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        logger.LogInformation("File {FileUrl} deleted", fileUrl);
        // In a real application, implement actual file deletion logic
        return Task.CompletedTask;
    }
}