using LigaD.Api.DTOs;
using LigaD.Api.Entidades;

namespace LigaD.Api.Repositorios;

/// <summary>
/// Catálogos de TLGD_TIP_VALOR.
/// </summary>
public interface ITipoValorRepositorio
{
    /// <summary>
    /// Códigos vigentes, de un grupo o de todos.
    /// SP: [dbo].[PRLGD_CON_TIP_VALOR]
    /// </summary>
    Task<RespuestaOperacionDto<IEnumerable<ETipoValor>>> ObtenerTodosPorSpAsync(string? grupo = null);
}
