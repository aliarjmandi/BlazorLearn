using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Abstractions;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

public sealed class SlideService : ISlideReadService, ISlideWriteService
{
    private readonly string _cs;
    public SlideService(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection")!;

    private IDbConnection Conn() => new SqlConnection(_cs);

    public async Task<IEnumerable<SlideDto>> GetActiveAsync(CancellationToken ct = default)
    {
        const string sql = @"
SELECT Id, Title, Caption, ImageUrl, LinkUrl, SortOrder, IsActive, StartAt, EndAt, CreatedAt
FROM dbo.Slides
WHERE IsActive = 1
  AND (StartAt IS NULL OR StartAt <= SYSUTCDATETIME())
  AND (EndAt   IS NULL OR EndAt   >= SYSUTCDATETIME())
ORDER BY SortOrder, CreatedAt DESC;";
        using var db = Conn();
        return await db.QueryAsync<SlideDto>(new CommandDefinition(sql, cancellationToken: ct));
    }

    public async Task<IEnumerable<SlideDto>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM dbo.Slides ORDER BY SortOrder, CreatedAt DESC;";
        using var db = Conn();
        return await db.QueryAsync<SlideDto>(new CommandDefinition(sql, cancellationToken: ct));
    }

    public async Task<SlideDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM dbo.Slides WHERE Id = @id;";
        using var db = Conn();
        return await db.QueryFirstOrDefaultAsync<SlideDto>(new CommandDefinition(sql, new { id }, cancellationToken: ct));
    }

    public async Task<Guid> CreateAsync(SlideDto dto, CancellationToken ct = default)
    {
        dto.Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id;
        const string sql = @"
INSERT INTO dbo.Slides(Id, Title, Caption, ImageUrl, LinkUrl, SortOrder, IsActive, StartAt, EndAt)
VALUES (@Id, @Title, @Caption, @ImageUrl, @LinkUrl, @SortOrder, @IsActive, @StartAt, @EndAt);";
        using var db = Conn();
        await db.ExecuteAsync(new CommandDefinition(sql, dto, cancellationToken: ct));
        return dto.Id;
    }

    public async Task<bool> UpdateAsync(SlideDto dto, CancellationToken ct = default)
    {
        const string sql = @"
UPDATE dbo.Slides SET
 Title=@Title, Caption=@Caption, ImageUrl=@ImageUrl, LinkUrl=@LinkUrl,
 SortOrder=@SortOrder, IsActive=@IsActive, StartAt=@StartAt, EndAt=@EndAt
WHERE Id=@Id;";
        using var db = Conn();
        var n = await db.ExecuteAsync(new CommandDefinition(sql, dto, cancellationToken: ct));
        return n > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM dbo.Slides WHERE Id=@id;";
        using var db = Conn();
        var n = await db.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: ct));
        return n > 0;
    }

    public async Task<bool> SetSortAsync(Guid id, int sortOrder, CancellationToken ct = default)
    {
        const string sql = "UPDATE dbo.Slides SET SortOrder=@sortOrder WHERE Id=@id;";
        using var db = Conn();
        var n = await db.ExecuteAsync(new CommandDefinition(sql, new { id, sortOrder }, cancellationToken: ct));
        return n > 0;
    }

    public async Task<bool> SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        const string sql = "UPDATE dbo.Slides SET IsActive=@isActive WHERE Id=@id;";
        using var db = Conn();
        var n = await db.ExecuteAsync(new CommandDefinition(sql, new { id, isActive }, cancellationToken: ct));
        return n > 0;
    }
}
