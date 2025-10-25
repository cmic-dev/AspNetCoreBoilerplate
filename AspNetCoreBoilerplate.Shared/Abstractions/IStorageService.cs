namespace AspNetCoreBoilerplate.Shared.Abstractions;

public interface IStorageService
{
    Task<string> UploadFileAsync(byte[] fileData, string fileName, string contentType, CancellationToken ctn = default);
}
