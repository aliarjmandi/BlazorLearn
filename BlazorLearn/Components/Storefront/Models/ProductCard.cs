namespace BlazorLearn.Components.Storefront.Models;

public record ProductCard(
    Guid Id,
    string Name,
    string Sku,
    decimal Price,
    int DiscountPercent,
    string? ThumbnailUrl
)
{
    public decimal FinalPrice => Price * (100 - DiscountPercent) / 100m;
}
