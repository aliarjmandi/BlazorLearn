namespace BlazorLearn.Components.Storefront.Models;

public sealed class SlideItem
{
    public string ImageUrl { get; set; } = default!;
    public string LinkUrl { get; set; } = default!;
    public string? Caption { get; set; }
}