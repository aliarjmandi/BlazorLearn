using Dapper;

namespace BlazorLearn.Services.Base
{
    public abstract class BaseService<TReadDto, TWriteDto, TKey>
        : BaseReadOnlyService<TReadDto, TKey>, IGenericService<TReadDto, TWriteDto, TKey>
    {
        protected BaseService(IConfiguration config) : base(config) { }

        // هر سرویس CRUD این‌ها را مشخص می‌کند
        protected abstract string SqlInsert { get; }
        protected abstract object GetInsertParams(TWriteDto dto);

        protected abstract string SqlUpdate { get; }
        protected abstract object GetUpdateParams(TKey id, TWriteDto dto);

        protected virtual string SqlDelete => "/* override if needed */";
        protected virtual object GetDeleteParams(TKey id) => new { Id = id };

        public virtual async Task CreateAsync(TWriteDto dto)
        {
            using var conn = GetConnection();
            await conn.ExecuteAsync(SqlInsert, GetInsertParams(dto));
        }

        public virtual async Task UpdateAsync(TKey id, TWriteDto dto)
        {
            using var conn = GetConnection();
            await conn.ExecuteAsync(SqlUpdate, GetUpdateParams(id, dto));
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            // اگر سرویس خاص SqlDelete نداد، پیش‌فرض با نام جدول باید override شود
            using var conn = GetConnection();
            await conn.ExecuteAsync(SqlDelete, GetDeleteParams(id));
        }
    }
}
