using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace BC2Gescom.Services;

public abstract class DataAccess
{
    protected readonly ConnectionStrings _appSettings;
    protected string _connectionString;

    protected DataAccess(IOptions<ConnectionStrings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    protected async Task<IEnumerable<T>> GetDbResultAsync<T>(string query)
    {
        using (var dbConn = new SqlConnection(_connectionString))
        {
            await dbConn.OpenAsync();

            return await dbConn.QueryAsync<T>(query);
        }
    }

    protected async Task<IEnumerable<T>> GetDbResultWithParameterAsync<T, V>(string query, V parameter)
    {
        using (var dbConn = new SqlConnection(_connectionString))
        {
            await dbConn.OpenAsync();

            return await dbConn.QueryAsync<T>(query, parameter);
        }
    }

    protected async Task RunQueryAsync(string query)
    {
        using (var dbConn = new SqlConnection(_connectionString))
        {
            await dbConn.OpenAsync();

            await dbConn.QueryAsync(query);
        }
    }

    protected async Task RunQueryWithParameterAsync<T>(string query, T parameter)
    {
        using (var dbConn = new SqlConnection(_connectionString))
        {
            await dbConn.OpenAsync();

            await dbConn.QueryAsync(query, parameter);
        }

    }
}
