namespace LigaD.Api.DTOs;

/// <summary>
/// Columnas tal como las devuelve PRLGD_CON_TIP_VALOR.
/// </summary>
public class TipoValorSpResultDto
{
    public string CTPV_GRUPO { get; set; } = string.Empty;
    public string CTPV_COD_VALOR { get; set; } = string.Empty;
    public string STPV_NOMBRE { get; set; } = string.Empty;
    public string? STPV_DESCRIPCION { get; set; }
    public int NTPV_ORDEN { get; set; }
    public string FTPV_ESTADO { get; set; } = string.Empty;
}
