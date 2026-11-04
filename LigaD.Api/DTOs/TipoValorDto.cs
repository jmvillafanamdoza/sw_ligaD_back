namespace LigaD.Api.DTOs;

/// <summary>
/// Código de catálogo que se expone al front (JSON en camelCase).
/// </summary>
public class TipoValorDto
{
    public string Grupo { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
}

/// <summary>
/// Filtro de GET /api/catalogos (query string).
/// </summary>
public class FiltroTipoValorDto
{
    /// <summary>Grupo del catálogo (TIPO_TORNEO, MEDALLA_DOTA, ESTADO_TORNEO...). Vacío = todos.</summary>
    public string? Grupo { get; set; }
}
