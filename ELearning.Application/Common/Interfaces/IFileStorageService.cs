namespace ELearning.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(byte[] content, string fileName, string contentType);

    Task<byte[]> GetFileAsync(string fileUrl);

    Task DeleteFileAsync(string fileUrl);
}