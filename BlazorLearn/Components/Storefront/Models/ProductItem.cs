namespace BlazorLearn.Components.Storefront.Models;

public sealed class ProductItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string LinkUrl { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int? DiscountPct { get; set; }
    public bool InStock { get; set; } = true;
}
