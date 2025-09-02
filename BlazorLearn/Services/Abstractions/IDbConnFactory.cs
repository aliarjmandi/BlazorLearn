using Microsoft.Data.SqlClient;
using System.Data;

namespace BlazorLearn.Services.Abstractions
{
    public interface IDbConnFactory
    {
        IDbConnection Create();
    }


}
