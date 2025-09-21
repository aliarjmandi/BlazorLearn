using BlazorLearn.Data.DTOs;

namespace BlazorLearn.Services.Abstractions
{
    public interface ISlideReadService
    {
        Task<IEnumerable<SlideDto>> GetActiveAsync(CancellationToken ct = default);
        Task<IEnumerable<SlideDto>> GetAllAsync(CancellationToken ct = default);
        Task<SlideDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }

}
