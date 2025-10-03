using Dapper;
using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

public sealed class BasalamRank 
{ public string? source { get; set; } public int parsedValue { get; set; } }
public sealed class BasalamNode
{
    public int id { get; set; }
    public string title { get; set; } = default!;
    public int parent_id { get; set; }
    public string slug { get; set; } = default!;
    public BasalamRank? rank { get; set; }
    public string? image_url { get; set; }
    public int level { get; set; }
    public List<BasalamNode> children { get; set; } = new();
}
public sealed class BasalamRoot { public List<BasalamNode> data { get; set; } = new(); }


public sealed class CategorySeeder
{
    private readonly IDbConnection _db;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpClientFactory _http;

    public CategorySeeder(IDbConnection db, IWebHostEnvironment env, IHttpClientFactory http)
    {
        _db = db;
        _env = env;
        _http = http;
    }

    public async Task SeedFromHtmlAsync(string relativeHtmlPath = "wwwroot/menu_final.html", bool dropAll = true)
    {
        // 1) load HTML + extract JSON
        var fullPath = Path.IsPathRooted(relativeHtmlPath)
            ? relativeHtmlPath
            : Path.Combine(_env.ContentRootPath, relativeHtmlPath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("menu_final.html not found.", fullPath);

        var html = await File.ReadAllTextAsync(fullPath);
        var m = Regex.Match(html, @"<script\s+id=""menuData""[^>]*>\s*(\{[\s\S]*?\})\s*</script>",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
        if (!m.Success) throw new InvalidOperationException("menuData JSON not found in html.");

        var root = JsonSerializer.Deserialize<BasalamRoot>(m.Groups[1].Value,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new BasalamRoot();

        // 2) clear table (Guid PK → reseed لازم نیست)
        if (dropAll)
            await _db.ExecuteAsync("DELETE FROM dbo.Categories;");

        // 3) insert recursively
        foreach (var l1 in root.data)
        {
            var id1 = await InsertAsync(null, l1);
            foreach (var l2 in l1.children)
            {
                var id2 = await InsertAsync(id1, l2);
                foreach (var l3 in l2.children)
                    _ = await InsertAsync(id2, l3);
            }
        }
    }

    private async Task<Guid> InsertAsync(Guid? parentId, BasalamNode n)
    {
        var id = Guid.NewGuid();
        var isLevel3 = n.level == 3;

        // دانلود تصویر (اختیاری، اگر URL دارد)
        string? iconUrl = null, imageUrl = null;
        if (!string.IsNullOrWhiteSpace(n.image_url))
        {
            var localPath = await DownloadToLocalAsync(n.image_url!, id, isLevel3 ? "image" : "icon");
            if (isLevel3) imageUrl = localPath; else iconUrl = localPath;
        }

        const string sql = @"
INSERT INTO dbo.Categories
    (Id, ParentId, Name, Slug, SortOrder, IsActive, IconUrl, ImageUrl, CreatedAt)
VALUES
    (@Id, @ParentId, @Name, @Slug, @SortOrder, 1, @IconUrl, @ImageUrl, SYSUTCDATETIME());";

        await _db.ExecuteAsync(sql, new
        {
            Id = id,
            ParentId = parentId,
            Name = n.title.Trim(),
            Slug = n.slug.Trim(),
            SortOrder = n.rank?.parsedValue ?? 0,
            IconUrl = iconUrl,
            ImageUrl = imageUrl
        });

        return id;
    }

    // دانلود و ذخیره: خروجی مسیر وب مثل "/uploads/categories/{id}/icon.webp"
    private async Task<string> DownloadToLocalAsync(string url, Guid id, string nameWithoutExt)
    {
        // normalize url (پروتکل-لس را https کن)
        if (url.StartsWith("//")) url = "https:" + url;

        var client = _http.CreateClient();
        using var resp = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        resp.EnsureSuccessStatusCode();

        // determine extension
        var ext = GetExtensionFromUrlOrContentType(url, resp.Content.Headers);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".bin";

        // build paths
        var relDir = Path.Combine("uploads", "categories", id.ToString("D"));
        var relPath = Path.Combine(relDir, $"{nameWithoutExt}{ext}").Replace("\\", "/");
        var fullDir = Path.Combine(_env.WebRootPath, relDir);
        Directory.CreateDirectory(fullDir);
        var fullPath = Path.Combine(_env.WebRootPath, relPath);

        await using (var fs = File.Create(fullPath))
        {
            await resp.Content.CopyToAsync(fs);
        }

        // مسیر وب
        return "/" + relPath.Replace("\\", "/");
    }

    private static string GetExtensionFromUrlOrContentType(string url, HttpContentHeaders headers)
    {
        // 1) از URL
        try
        {
            var u = new Uri(url, UriKind.Absolute);
            var ext = Path.GetExtension(u.AbsolutePath);
            if (!string.IsNullOrWhiteSpace(ext)) return ext.ToLowerInvariant();
        }
        catch { /* ignore */ }

        // 2) از Content-Type
        var ct = headers.ContentType?.MediaType?.ToLowerInvariant();
        return ct switch
        {
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/jpeg" => ".jpg",
            "image/svg+xml" => ".svg",
            "image/avif" => ".avif",
            _ => ".bin"
        };
    }
}
