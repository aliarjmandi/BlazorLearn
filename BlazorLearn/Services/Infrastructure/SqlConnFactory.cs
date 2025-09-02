using BlazorLearn.Services.Abstractions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BlazorLearn.Services.Infrastructure
{
    public class SqlConnFactory : IDbConnFactory
    {
        private readonly string _cs;
        public SqlConnFactory(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection");
        public IDbConnection Create() => new SqlConnection(_cs);
    }
}
