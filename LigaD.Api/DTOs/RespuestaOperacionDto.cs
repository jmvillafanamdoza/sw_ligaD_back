namespace LigaD.Api.DTOs;

/// <summary>
/// Resultado de una operación de repositorio: esExitoso, mensaje, datos.
/// </summary>
public class RespuestaOperacionDto<T>
{
    public bool EsExitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public T? Datos { get; set; }

    public static RespuestaOperacionDto<T> Exitoso(T? datos, string mensaje = "Operación exitosa") => new()
    {
        EsExitoso = true,
        Mensaje = mensaje,
        Datos = datos
    };

    public static RespuestaOperacionDto<T> Error(string mensaje) => new()
    {
        EsExitoso = false,
        Mensaje = mensaje,
        Datos = default
    };
}
