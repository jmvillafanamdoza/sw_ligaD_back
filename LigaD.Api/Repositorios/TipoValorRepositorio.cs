using System.Data;
using Dapper;
using LigaD.Api.DTOs;
using LigaD.Api.Entidades;

namespace LigaD.Api.Repositorios;

/// <summary>
/// Repositorio de catálogos con Dapper + stored procedures.
/// </summary>
public class TipoValorRepositorio(IConnectionFactory connectionFactory, ILogger<TipoValorRepositorio> logger)
    : ITipoValorRepositorio
{
    public async Task<RespuestaOperacionDto<IEnumerable<ETipoValor>>> ObtenerTodosPorSpAsync(string? grupo = null)
    {
        try
        {
            using var connection = connectionFactory.CreateConnection();
            const string spName = "[dbo].[PRLGD_CON_TIP_VALOR]";

            var parametros = new DynamicParameters();
            parametros.Add("@i_CTPV_GRUPO", string.IsNullOrWhiteSpace(grupo) ? null : grupo.Trim().ToUpperInvariant());
            parametros.Add("@i_FTPV_ESTADO", "V");

            var resultado = await connection.QueryAsync<TipoValorSpResultDto>(
                spName, parametros, commandType: CommandType.StoredProcedure);

            var datos = resultado.Select(r => new ETipoValor
            {
                Grupo = r.CTPV_GRUPO,
                Codigo = r.CTPV_COD_VALOR,
                Nombre = r.STPV_NOMBRE,
                Descripcion = r.STPV_DESCRIPCION,
                Orden = r.NTPV_ORDEN,
                Estado = r.FTPV_ESTADO
            }).ToList();

            if (datos.Count == 0)
            {
                return RespuestaOperacionDto<IEnumerable<ETipoValor>>.Error(
                    string.IsNullOrWhiteSpace(grupo)
                        ? "No hay catálogos cargados."
                        : $"No existe el catálogo '{grupo}'.");
            }

            return RespuestaOperacionDto<IEnumerable<ETipoValor>>.Exitoso(
                datos, $"Se obtuvieron {datos.Count} códigos.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al consultar PRLGD_CON_TIP_VALOR (grupo {Grupo})", grupo);
            return RespuestaOperacionDto<IEnumerable<ETipoValor>>.Error("Error al obtener los catálogos.");
        }
    }
}
