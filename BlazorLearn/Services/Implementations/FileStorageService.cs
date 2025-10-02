using Microsoft.AspNetCore.Components.Forms;

namespace BlazorLearn.Services.Implementations;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    public FileStorageService(IWebHostEnvironment env) => _env = env;

    // متد قبلی برای پروفایل‌ها
    public async Task<string> SaveProfileAsync(byte[] content, string fileName)
    {
        var uploads = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploads);

        var safe = Path.GetFileName(fileName);
        var unique = $"{Path.GetFileNameWithoutExtension(safe)}_{Guid.NewGuid():N}{Path.GetExtension(safe)}";
        var path = Path.Combine(uploads, unique);

        await File.WriteAllBytesAsync(path, content);
        return $"/uploads/{unique}";
    }

    // متد جدید برای آپلود مستقیم از InputFile
    public async Task<string> SaveAsync(IBrowserFile file, string relativePath, CancellationToken ct = default)
    {
        relativePath = relativePath.Replace("\\", "/").TrimStart('/');
        var fullPath = Path.Combine(_env.WebRootPath, relativePath);

        var dir = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(dir);

        const long maxSize = 10 * 1024 * 1024; // حداکثر 10MB
        await using var read = file.OpenReadStream(maxSize, ct);
        await using var write = File.Create(fullPath);
        await read.CopyToAsync(write, ct);

        return "/" + relativePath;
    }
}
