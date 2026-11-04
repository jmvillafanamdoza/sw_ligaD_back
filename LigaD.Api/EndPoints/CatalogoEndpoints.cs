using FluentValidation;
using LigaD.Api.DTOs;
using LigaD.Api.Repositorios;
using LigaD.Api.Utilitarios;

namespace LigaD.Api.EndPoints;

/// <summary>
/// Catálogos de TLGD_TIP_VALOR (tipos de torneo, medallas, roles, estados...). Público.
/// </summary>
public static class CatalogoEndpoints
{
    public static void MapCatalogoEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/api/catalogos")
                       .WithTags(Constantes.Grupos.Catalogos);

        // GET /api/catalogos?grupo=TIPO_TORNEO  (sin grupo: todos los vigentes)
        grupo.MapGet("/", async (
            [AsParameters] FiltroTipoValorDto filtro,
            IValidator<FiltroTipoValorDto> validator,
            ITipoValorRepositorio repo) =>
        {
            var validacion = await validator.ValidateAsync(filtro);
            if (!validacion.IsValid)
                return RespuestaHttp.ErrorValidacion(validacion.Errors.Select(e => e.ErrorMessage));

            var respuestaRepo = await repo.ObtenerTodosPorSpAsync(filtro.Grupo);
            if (!respuestaRepo.EsExitoso)
                return RespuestaHttp.NoEncontrado(respuestaRepo.Mensaje);

            var dtos = respuestaRepo.Datos!.Select(e => new TipoValorDto
            {
                Grupo = e.Grupo,
                Codigo = e.Codigo,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                Orden = e.Orden
            });

            return RespuestaHttp.Ok(dtos, respuestaRepo.Mensaje);
        })
        .WithName("ObtenerCatalogos")
        .Produces<IEnumerable<TipoValorDto>>();
    }
}
