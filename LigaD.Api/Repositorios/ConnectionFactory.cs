using System.Data;
using LigaD.Api.Config;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace LigaD.Api.Repositorios;

public class ConnectionFactory(IOptions<DatabaseSettings> settings) : IConnectionFactory
{
    private readonly string _connectionString = settings.Value.DefaultConnection;

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
