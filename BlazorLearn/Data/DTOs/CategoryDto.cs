using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearn.Data.DTOs
{
    // مستقیماً مپ می‌شود به جدول Categories (Dapper نیازی به اتربیوت ندارد)
    public partial class CategoryDto
    {
        [Required]
        public Guid Id { get; set; }            // PK (GUID)
        public Guid? ParentId { get; set; }     // FK به Category.Id (nullable برای ریشه)

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(120)]
        public string Slug { get; set; } = default!;

        [Required]
        public int SortOrder { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
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

        public int SortOrder { get; set; } = 0;      // پیش‌فرض آخر لیست
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; }
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

        public List<CategoryTreeNodeDto> Children { get; set; } = new();
    }
}
