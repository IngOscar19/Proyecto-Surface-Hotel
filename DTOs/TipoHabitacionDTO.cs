using System.ComponentModel.DataAnnotations;

namespace Hotel.DTOs
{
    public class TipoHabitacionDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El factor de tipo es requerido")]
        [Range(0.1, 10.0, ErrorMessage = "El factor debe estar entre 0.1 y 10.0")]
        public decimal FactorTipo { get; set; }
    }
}