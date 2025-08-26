using BlazorLearn.Data.DTOs;
using BlazorLearn.Services.Base;

namespace BlazorLearn.Services.Implementations
{
    public partial class CategoryService
        : BaseService<CategoryDto, CategoryWriteDto, Guid>
    {
        public CategoryService(IConfiguration config) : base(config) { }

        // ---- READ ----
        protected override string SqlSelectAll => @"
            SELECT 
                c.Id,
                c.ParentId,
                c.Name,
                c.Slug,
                c.SortOrder,
                c.IsActive,
                c.CreatedAt
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
                c.CreatedAt
            FROM dbo.Categories AS c
            WHERE c.Id = @Id
        ";

        // ترتیب پیش‌فرض برای GetAll
        protected override string SqlOrderBy => "SortOrder ASC";

        // ---- WRITE ----
        protected override string SqlInsert => @"
            INSERT INTO dbo.Categories
            (ParentId, Name, Slug, SortOrder, IsActive, CreatedAt)
            VALUES
            (@ParentId, @Name, @Slug, @SortOrder, @IsActive, @CreatedAt);
        ";

        protected override object GetInsertParams(CategoryWriteDto dto)
            => new
            {
                dto.ParentId,
                dto.Name,
                dto.Slug,
                dto.SortOrder,
                dto.IsActive,
                dto.CreatedAt
            };

        protected override string SqlUpdate => @"
            UPDATE dbo.Categories
               SET ParentId = @ParentId,
                   Name     = @Name,
                   Slug     = @Slug,
                   SortOrder= @SortOrder,
                   IsActive = @IsActive
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
                dto.IsActive
            };

        protected override string SqlDelete => "DELETE FROM dbo.Categories WHERE Id=@Id";
    }
}
