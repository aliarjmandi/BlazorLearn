using Microsoft.AspNetCore.Components.Forms;

public interface IFileStorageService
{
    Task<string> SaveProfileAsync(byte[] content, string fileName);
    Task<string> SaveAsync(IBrowserFile file, string relativePath, CancellationToken ct = default);
}