using System.Threading.Tasks;

namespace BlazorLearn.Services.Base
{
    public interface IGenericService<TReadDto, TWriteDto, TKey>
        : IReadOnlyService<TReadDto, TKey>
    {
        Task CreateAsync(TWriteDto dto);
        Task UpdateAsync(TKey id, TWriteDto dto);
        Task DeleteAsync(TKey id);
    }
}