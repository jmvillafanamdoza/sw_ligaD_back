using LigaD.Api.Utilitarios;

namespace LigaD.Api.EndPoints;

/// <summary>
/// Comprobación de que el API está arriba (no toca la base).
/// </summary>
public static class SaludEndpoints
{
    public record SaludDto(string Servicio, string Entorno, DateTimeOffset Fecha);

    public static void MapSaludEndpoints(this WebApplication app)
    {
        app.MapGet("/api/salud", (IHostEnvironment entorno) =>
                RespuestaHttp.Ok(new SaludDto("Liga D API", entorno.EnvironmentName, DateTimeOffset.UtcNow)))
           .WithTags(Constantes.Grupos.Salud)
           .WithName("Salud")
           .Produces<SaludDto>();
    }
}
