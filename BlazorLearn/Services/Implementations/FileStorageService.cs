namespace BlazorLearn.Services.Implementations;

public class FileStorageService
{
    private readonly IWebHostEnvironment _env;
    public FileStorageService(IWebHostEnvironment env) => _env = env;

    public async Task<string> SaveProfileAsync(byte[] content, string fileName)
    {
        var uploads = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploads);

        var safe = Path.GetFileName(fileName);
        var unique = $"{Path.GetFileNameWithoutExtension(safe)}_{Guid.NewGuid():N}{Path.GetExtension(safe)}";
        var path = Path.Combine(uploads, unique);

        await File.WriteAllBytesAsync(path, content);
        return $"/uploads/{unique}"; // مسیر وب
    }
}
