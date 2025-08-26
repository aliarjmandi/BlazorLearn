// Tag
using System.ComponentModel.DataAnnotations;

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class TagWriteDto
{
    public Guid? Id { get; set; }
    [Required, MaxLength(50)] public string Name { get; set; } = default!;
    [MaxLength(80)] public string? Slug { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

// ProductTag (خواندن)
public class ProductTagDto
{
    public Guid ProductId { get; set; }
    public Guid TagId { get; set; }
    public string TagName { get; set; } = default!;
}
