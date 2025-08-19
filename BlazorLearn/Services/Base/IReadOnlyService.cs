namespace BlazorLearn.Services.Base
{
    //ارسال پارامتر به داخل اینترفیس
    public interface IReadOnlyService<TReadDto, TKey> 
    {
        Task<IEnumerable<TReadDto>> GetAllAsync();
        Task<TReadDto?> GetByIdAsync(TKey id);
        //ساختار تاپل برای بازیابی اطلاعات
        Task<(IEnumerable<TReadDto> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    }
}
