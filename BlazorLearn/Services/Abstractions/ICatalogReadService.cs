using BlazorLearn.Data.DTOs;

namespace BlazorLearn.Services.Abstractions;

public interface ICatalogReadService
{
    Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

    Task<IEnumerable<SlideDto>> GetSlidesAsync();
    Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int take = 10);

    Task<ProductDto?> GetProductBySlugAsync(string slug);
    Task<IEnumerable<ProductImageDto>> GetProductImagesAsync(Guid productId);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId, int take = 12);

    // صفحه‌بندی دسته (که قبل‌تر اضافه کردیم)
    Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetProductsByCategoryPagedAsync(
        string categorySlug, int page, int pageSize, string sort = "new");

    // ⬅️ جدید: مسیر نان‌برگرام دسته از ریشه تا برگ
    Task<List<CategoryDto>> GetCategoryPathAsync(Guid categoryId);

    Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword, int take = 20);
}
