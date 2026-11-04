using Microsoft.OpenApi.Models;

namespace LigaD.Api.Config;

/// <summary>
/// Documento OpenAPI nativo (.NET 9). La UI la sirve Scalar en /scalar/v1 (solo Development).
/// </summary>
public static class OpenApiConfiguracion
{
    public static IServiceCollection AddLigaDOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Liga D API",
                    Version = "v1",
                    Description = "API de Liga D: torneos amateur de Dota 2 (Perú)."
                };
                return Task.CompletedTask;
            });
        });

        return services;
    }
}
