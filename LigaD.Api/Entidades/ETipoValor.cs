namespace LigaD.Api.Entidades;

/// <summary>
/// Código de catálogo (tabla TLGD_TIP_VALOR).
/// </summary>
public class ETipoValor
{
    /// <summary>CTPV_GRUPO</summary>
    public string Grupo { get; set; } = string.Empty;

    /// <summary>CTPV_COD_VALOR</summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>STPV_NOMBRE</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>STPV_DESCRIPCION</summary>
    public string? Descripcion { get; set; }

    /// <summary>NTPV_ORDEN</summary>
    public int Orden { get; set; }

    /// <summary>FTPV_ESTADO (V vigente, I inactivo, A anulado)</summary>
    public string Estado { get; set; } = string.Empty;
}
