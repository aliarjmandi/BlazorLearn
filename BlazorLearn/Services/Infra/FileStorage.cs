using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorLearn.Services.Infra
{
    public interface IFileStorage
    {
        Task<string> SaveProductImageAsync(Guid productId, IBrowserFile file, CancellationToken ct = default);
        Task DeleteAsync(string relativePath);
        bool Exists(string relativePath);
    }

    public class FileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment _env;
        public FileStorage(IWebHostEnvironment env) => _env = env;

        private string WebRoot => _env.WebRootPath;

        public async Task<string> SaveProductImageAsync(Guid productId, IBrowserFile file, CancellationToken ct = default)
        {
            // محدودیت حجم (مثلاً 5MB)
            const long maxBytes = 5 * 1024 * 1024;
            var safeName = Path.GetFileNameWithoutExtension(file.Name);
            var ext = Path.GetExtension(file.Name).ToLowerInvariant();

            // فقط تصاویر مجاز
            if (ext is not (".jpg" or ".jpeg" or ".png" or ".webp"))
                throw new InvalidOperationException("فرمت تصویر معتبر نیست.");

            var dir = Path.Combine(WebRoot, "uploads", "products", productId.ToString("N"));
            Directory.CreateDirectory(dir);

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(dir, fileName);

            await using var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await using var stream = file.OpenReadStream(maxBytes, ct);
            await stream.CopyToAsync(fs, ct);

            // مسیر نسبی برای استفاده در <img src="/...">
            var rel = $"/uploads/products/{productId:N}/{fileName}";
            return rel.Replace("\\", "/");
        }

        public Task DeleteAsync(string relativePath)
        {
            var full = Path.Combine(WebRoot, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(full)) File.Delete(full);
            return Task.CompletedTask;
        }

        public bool Exists(string relativePath)
        {
            var full = Path.Combine(WebRoot, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            return File.Exists(full);
        }
    }
}
