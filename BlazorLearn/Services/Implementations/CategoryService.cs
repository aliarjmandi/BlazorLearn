using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Base;

namespace BlazorLearn.Services.Implementations
{
    public partial class CategoryService
        : BaseService<CategoryDto, CategoryWriteDto, Guid>
    {
        public CategoryService(IConfiguration config) : base(config) { }

        // ---- READ ----
        // ---- READ ----
        protected override string SqlSelectAll => @"
    SELECT 
        c.Id,
        c.ParentId,
        c.Name,
        c.Slug,
        c.SortOrder,
        c.IsActive,
        c.CreatedAt,
        c.ImageUrl,        -- 👈 جدید
        c.IconUrl          -- 👈 جدید
    FROM dbo.Categories AS c
";

        protected override string SqlSelectById => @"
    SELECT 
        c.Id,
        c.ParentId,
        c.Name,
        c.Slug,
        c.SortOrder,
        c.IsActive,
        c.CreatedAt,
        c.ImageUrl,        -- 👈 جدید
        c.IconUrl          -- 👈 جدید
    FROM dbo.Categories AS c
    WHERE c.Id = @Id
";

        // ترتیب پیش‌فرض برای GetAll
        protected override string SqlOrderBy => "SortOrder ASC";

        // ---- WRITE ----
        // ---- WRITE ----
        protected override string SqlInsert => @"
    INSERT INTO dbo.Categories
    (Id, ParentId, Name, Slug, SortOrder, IsActive, CreatedAt, ImageUrl, IconUrl)
    VALUES
    (@Id, @ParentId, @Name, @Slug, @SortOrder, @IsActive, @CreatedAt, @ImageUrl, @IconUrl);
";

        protected override object GetInsertParams(CategoryWriteDto dto)
            => new
            {
                Id = dto.Id ?? Guid.NewGuid(),
                dto.ParentId,
                dto.Name,
                dto.Slug,
                dto.SortOrder,
                dto.IsActive,
                dto.CreatedAt,
                dto.ImageUrl,     // 👈 جدید
                dto.IconUrl       // 👈 جدید
            };

        protected override string SqlUpdate => @"
    UPDATE dbo.Categories
       SET ParentId = @ParentId,
           Name     = @Name,
           Slug     = @Slug,
           SortOrder= @SortOrder,
           IsActive = @IsActive,
           ImageUrl = @ImageUrl,   -- 👈 جدید
           IconUrl  = @IconUrl     -- 👈 جدید
     WHERE Id = @Id;
";
        protected override object GetUpdateParams(Guid id, CategoryWriteDto dto)
            => new
            {
                Id = id,
                dto.ParentId,
                dto.Name,
                dto.Slug,
                dto.SortOrder,
                dto.IsActive,
                dto.ImageUrl,     // 👈 جدید
                dto.IconUrl       // 👈 جدید
            };


        protected override string SqlDelete => "DELETE FROM dbo.Categories WHERE Id=@Id";
    }
}
