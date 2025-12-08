using FluentValidation;
using static Hotel.Controllers.UsuariosController;
using ProjectHotel.DTOs;

namespace Hotel.Validators
{
    public class RegistroRequestValidator : AbstractValidator<RegistroRequest>
    {
        public RegistroRequestValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(150).WithMessage("El nombre no puede exceder 150 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El nombre solo puede contener letras");

            RuleFor(x => x.Apellido)
                .NotEmpty().WithMessage("El apellido es obligatorio")
                .MaximumLength(150).WithMessage("El apellido no puede exceder 150 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El apellido solo puede contener letras");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El email no es válido")
                .MaximumLength(150).WithMessage("El email no puede exceder 150 caracteres");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
                .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
                .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
                .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número")
                .Matches(@"^[A-Za-z0-9!@#$%^&*()_\-+=\[\]{}.,:;?<>/\\|]+$")
                    .WithMessage("La contraseña contiene caracteres no permitidos");


            RuleFor(x => x.Rol)
                .NotEmpty().WithMessage("El rol es obligatorio")
                .Must(BeAValidRole).WithMessage("El rol debe ser: admin, empleado o cliente");
        }

        private bool BeAValidRole(string rol)
        {
            var rolesValidos = new[] { "admin", "empleado", "cliente" };
            return rolesValidos.Contains(rol.ToLower());
        }
    }
}