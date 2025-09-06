using BlazorLearn.Data.DTOs;

namespace BlazorLearn.Services.Abstractions;

public interface ICatalogReadService
{
    Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();   // قبلی: فقط ریشه‌ها (برای لندینگ)
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();    // جدید: همه دسته‌ها برای مگا منو

    Task<IEnumerable<SlideDto>> GetSlidesAsync();
    Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int take = 10);
}
