using FluentValidation;
using ProjectHotel.DTOs;

namespace Hotel.Validators
{
    public class CrearHabitacionValidator : AbstractValidator<CrearHabitacionRequest>
    {
        public CrearHabitacionValidator()
        {
            RuleFor(x => x.NumeroHabitacion)
                .NotEmpty().WithMessage("El número de habitación es obligatorio")
                .MaximumLength(10).WithMessage("El número no puede exceder 10 caracteres")
                .Matches(@"^[a-zA-Z0-9-]+$").WithMessage("El número solo puede contener letras, números y guiones");

            RuleFor(x => x.TipoHabitacionId)
                .GreaterThan(0).WithMessage("Debe seleccionar un tipo de habitación válido");

            RuleFor(x => x.PrecioBase)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0")
                .LessThanOrEqualTo(100000).WithMessage("El precio no puede exceder 100,000");

            RuleFor(x => x.Capacidad)
                .GreaterThan((short)0).WithMessage("La capacidad debe ser mayor a 0")
                .LessThanOrEqualTo((short)10).WithMessage("La capacidad no puede exceder 10 personas");

            RuleFor(x => x.Descripcion)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.Piso)
                .GreaterThanOrEqualTo((short)0).WithMessage("El piso debe ser mayor o igual a 0")
                .LessThanOrEqualTo((short)50).WithMessage("El piso no puede exceder 50");
        }
    }

    public class ActualizarHabitacionValidator : AbstractValidator<ActualizarHabitacionRequest>
{
    public ActualizarHabitacionValidator()
    {
        RuleFor(x => x.NumeroHabitacion)
            .MaximumLength(10).WithMessage("El número de habitación no puede exceder 10 caracteres")
            .When(x => !string.IsNullOrEmpty(x.NumeroHabitacion));

        RuleFor(x => x.TipoHabitacionId)
            .GreaterThan(0).WithMessage("El tipo de habitación debe ser válido")
            .When(x => x.TipoHabitacionId.HasValue);

        RuleFor(x => x.Piso)
            .GreaterThanOrEqualTo((short)0).WithMessage("El piso debe ser mayor o igual a 0")
            .LessThanOrEqualTo((short)50).WithMessage("El piso no puede exceder 50")
            .When(x => x.Piso.HasValue);

        RuleFor(x => x.PrecioBase)
            .GreaterThan(0).WithMessage("El precio base debe ser mayor a 0")
            .LessThanOrEqualTo(10000).WithMessage("El precio base no puede exceder 10000")
            .When(x => x.PrecioBase.HasValue);

        RuleFor(x => x.Capacidad)
            .GreaterThan((short)0).WithMessage("La capacidad debe ser mayor a 0")
            .LessThanOrEqualTo((short)20).WithMessage("La capacidad no puede exceder 20")
            .When(x => x.Capacidad.HasValue);

        RuleFor(x => x.Estado)
            .Must(estado => new[] { "disponible", "ocupada", "mantenimiento", "limpieza" }
                .Contains(estado.ToLower()))
            .WithMessage("Estado inválido. Debe ser: disponible, ocupada, mantenimiento o limpieza")
            .When(x => !string.IsNullOrEmpty(x.Estado));

        // ✅ Validaciones para fotos
        RuleForEach(x => x.Fotos).ChildRules(foto =>
        {
            foto.RuleFor(f => f.Url)
                .NotEmpty().WithMessage("La URL de la foto es requerida")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("La URL de la foto debe ser válida");

            foto.RuleFor(f => f.Descripcion)
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres");
        }).When(x => x.Fotos != null && x.Fotos.Any());
    }

        private bool BeAValidEstado(string estado)
        {
            var estadosValidos = new[] { "disponible", "ocupada", "mantenimiento", "limpieza" };
            return estadosValidos.Contains(estado.ToLower());
        }
    }

    public class AgregarFotoValidator : AbstractValidator<AgregarFotoRequest>
    {
        public AgregarFotoValidator()
        {
            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("La URL es obligatoria")
                .MaximumLength(255).WithMessage("La URL no puede exceder 255 caracteres")
                .Must(BeAValidUrl).WithMessage("Debe ser una URL válida");

            RuleFor(x => x.Descripcion)
                .MaximumLength(255).WithMessage("La descripción no puede exceder 255 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}