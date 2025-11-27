using System.ComponentModel.DataAnnotations;

namespace Hotel.DTOs

{


    public class TemporadaPrecioDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public DateTime FechaFin { get; set; }

        [Range(0.01, 10.00, ErrorMessage = "El factor multiplicador debe estar entre 0.01 y 10.00")]
        public decimal FactorMultiplicador { get; set; } = 1.00m;

        public bool Activo { get; set; } = true;
    }

    public class TemporadaHabitacionPrecioDto
    {
        [Required(ErrorMessage = "El ID de la temporada es requerido")]
        public int TemporadaId { get; set; }

        [Required(ErrorMessage = "El ID de la habitación es requerido")]
        public int HabitacionId { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
        public decimal PrecioOverride { get; set; }
    }

    public class TemporadaHabitacionPrecioCreateMultipleDto
    {
        [Required(ErrorMessage = "El ID de la habitación es requerido")]
        public int HabitacionId { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
        public decimal PrecioOverride { get; set; }
    }
}