namespace LigaD.Api.Utilitarios;

/// <summary>
/// Lectura del @o_resultMessage de los SPs de escritura ("1|mensaje" o "0|error").
/// </summary>
public static class RespuestaSp
{
    public static (bool EsExitoso, string Mensaje) Parsear(string? respuesta)
    {
        if (string.IsNullOrEmpty(respuesta))
            return (false, "Sin respuesta de la base de datos.");

        var separador = respuesta.IndexOf('|');
        if (separador < 0)
            return (respuesta == "1", string.Empty);

        return (respuesta[..separador] == "1", respuesta[(separador + 1)..]);
    }
}
