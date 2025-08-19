using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorLearn.Services.Base
{
    public abstract class BaseReadOnlyService<TReadDto, TKey> : IReadOnlyService<TReadDto, TKey>
    {
        private readonly IConfiguration _config;

        protected BaseReadOnlyService(IConfiguration config)
        {
            _config = config;
        }

        protected SqlConnection GetConnection()
            => new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        // هر سرویس این‌ها را مشخص می‌کند
        protected abstract string SqlSelectAll { get; }         // بدون ORDER BY
        protected abstract string SqlSelectById { get; }        // WHERE Id=@Id
        protected virtual string SqlOrderBy => "Id";            // فیلد مرتب‌سازی پیش‌فرض

        public virtual async Task<IEnumerable<TReadDto>> GetAllAsync()
        {
            using var conn = GetConnection();
            var sql = $"{SqlSelectAll} ORDER BY {SqlOrderBy}";
            return await conn.QueryAsync<TReadDto>(sql);
        }

        public virtual async Task<TReadDto?> GetByIdAsync(TKey id)
        {
            using var conn = GetConnection();
            return await conn.QueryFirstOrDefaultAsync<TReadDto>(SqlSelectById, new { Id = id });
        }

        public virtual async Task<(IEnumerable<TReadDto> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            using var conn = GetConnection();

            var dataSql = $@"
            SELECT * FROM (
                {SqlSelectAll}
            ) AS Q
            ORDER BY {SqlOrderBy}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var countSql = $"SELECT COUNT(1) FROM ({SqlSelectAll}) AS Q;";

            var items = await conn.QueryAsync<TReadDto>(dataSql, new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            });
            var total = await conn.ExecuteScalarAsync<int>(countSql);

            return (items, total);
        }
    }
}
