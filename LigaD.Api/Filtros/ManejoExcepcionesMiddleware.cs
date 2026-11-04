using LigaD.Api.DTOs.Common;
using LigaD.Api.Utilitarios;

namespace LigaD.Api.Filtros;

/// <summary>
/// Captura cualquier excepción no controlada y responde con el envoltorio estándar
/// en vez de un stack trace.
/// </summary>
public class ManejoExcepcionesMiddleware(RequestDelegate next, ILogger<ManejoExcepcionesMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Excepción no controlada procesando {Path}", context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(RespuestaApiDto<object>.Error(Constantes.Mensajes.ErrorInterno));
        }
    }
}
