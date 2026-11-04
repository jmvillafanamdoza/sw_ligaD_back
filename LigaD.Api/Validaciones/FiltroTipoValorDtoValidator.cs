using FluentValidation;
using LigaD.Api.DTOs;

namespace LigaD.Api.Validaciones;

public class FiltroTipoValorDtoValidator : AbstractValidator<FiltroTipoValorDto>
{
    public FiltroTipoValorDtoValidator()
    {
        RuleFor(x => x.Grupo)
            .MaximumLength(30).WithMessage("El grupo admite como máximo 30 caracteres.")
            .Matches("^[A-Za-z_]*$").WithMessage("El grupo solo admite letras y guion bajo.");
    }
}
