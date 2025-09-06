using BlazorLearn.Data.DTOs;

namespace BlazorLearn.Services.Abstractions;

public interface ICatalogReadService
{
    /// اسلایدهای هدر – فعلاً ماک
    Task<IEnumerable<SlideDto>> GetSlidesAsync();

    /// فقط دسته‌های ریشه (برای منوی لندینگ)
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync();

    /// چند محصول پیشنهادی/آخرین‌ها به همراه اولین تصویر
    Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int take = 10);
}
