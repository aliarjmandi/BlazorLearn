using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.DTOs
{
    // مستقیماً مپ می‌شود به جدول Categories (Dapper نیازی به اتربیوت ندارد)
    public partial class CategoryDto
    {
        [Required] public Guid Id { get; set; }
        public Guid? ParentId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(120)]
        public string Slug { get; set; } = default!;

        // جدید
        [MaxLength(400)] public string? ImageUrl { get; set; }   // تصویر کارت/بنر (برای سطح 3 و کارت‌ها)
        [MaxLength(400)] public string? IconUrl { get; set; }   // آیکن کوچک (برای سطح 1/2)

        [Required] public int SortOrder { get; set; }
        [Required] public bool IsActive { get; set; }
        [Required] public DateTime CreatedAt { get; set; }
    }

    // برای سناریوهای ایجاد/ویرایش (ورودی فرم‌ها) — از Id/CreatedAt جدا می‌شه
    public class CategoryWriteDto
    {
        public Guid? Id { get; set; }
        public Guid? ParentId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(120)]
        public string Slug { get; set; } = default!;

        // جدید
        [MaxLength(400)] public string? ImageUrl { get; set; }
        [MaxLength(400)] public string? IconUrl { get; set; }

        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // اگر ستون CreatedAt در DB مقدار پیش‌فرض دارد، می‌توانی این فیلد را از WriteDto حذف کنی.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // برای حرکت نود (Drag&Drop) — آپدیت ParentId و SortOrder
    public class CategoryMoveDto
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? NewParentId { get; set; }
        [Required]
        public int NewSortOrder { get; set; }
    }

    // برای مرتب‌سازی یک سطح (Sibling ها) — وقتی چند آیتم جابه‌جا شده‌اند
    public class CategoryReorderDto
    {
        public Guid? ParentId { get; set; }          // سطح هدف (null = ریشه)
        [Required]
        public List<Guid> OrderedIds { get; set; } = new();
    }

    // برای ارسال/نمایش درخت در UI (ViewModel سمت کلاینت)
    // توجه: این کلاس را مستقیم در DB ننشانید—ویژه‌ی UI است.
    public class CategoryTreeNodeDto
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }

        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }

        // جدید برای نمایش در منو
        public string? ImageUrl { get; set; }
        public string? IconUrl { get; set; }

        public List<CategoryTreeNodeDto> Children { get; set; } = new();
    }
}
