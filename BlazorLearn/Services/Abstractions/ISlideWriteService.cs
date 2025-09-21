using BlazorLearn.Data.DTOs;

namespace BlazorLearn.Services.Abstractions
{
    public interface ISlideWriteService
    {
        Task<Guid> CreateAsync(SlideDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(SlideDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
        Task<bool> SetSortAsync(Guid id, int sortOrder, CancellationToken ct = default);
        Task<bool> SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default);
    }

}
