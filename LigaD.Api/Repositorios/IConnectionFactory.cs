using System.Data;

namespace LigaD.Api.Repositorios;

/// <summary>
/// Fábrica de conexiones a BD_LIGAD que usan los repositorios con Dapper.
/// </summary>
public interface IConnectionFactory
{
    IDbConnection CreateConnection();
}
