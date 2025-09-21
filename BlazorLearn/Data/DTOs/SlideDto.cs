namespace BlazorLearn.Data.DTOs;
public sealed class SlideDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Caption { get; set; }
    public string ImageUrl { get; set; } = default!;
    public string? LinkUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
