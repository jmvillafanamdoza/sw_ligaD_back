using LigaD.Api.DTOs.Common;

namespace LigaD.Api.Utilitarios;

/// <summary>
/// IResult consistentes para los endpoints (siempre con el envoltorio RespuestaApiDto).
/// </summary>
public static class RespuestaHttp
{
    public static IResult Ok<T>(T datos, string? mensaje = null) =>
        Results.Ok(RespuestaApiDto<T>.Ok(datos, mensaje));

    public static IResult Creado<T>(string uri, T datos, string? mensaje = null) =>
        Results.Created(uri, RespuestaApiDto<T>.Ok(datos, mensaje));

    public static IResult NoEncontrado(string mensaje = "Recurso no encontrado.") =>
        Results.NotFound(RespuestaApiDto<object>.Error(mensaje));

    public static IResult ErrorValidacion(IEnumerable<string> errores) =>
        Results.BadRequest(RespuestaApiDto<object>.Error(Constantes.Mensajes.ErrorValidacion, errores));

    public static IResult ErrorInterno() =>
        Error(StatusCodes.Status500InternalServerError, Constantes.Mensajes.ErrorInterno);

    public static IResult Error(int codigo, string mensaje) =>
        Results.Json(RespuestaApiDto<object>.Error(mensaje), statusCode: codigo);
}
