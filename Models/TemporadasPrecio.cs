using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Hotel.Models
{
    public class TemporadaPrecio
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Required]
        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Column("fecha_fin")]
        public DateTime FechaFin { get; set; }

        [Column("factor_multiplicador", TypeName = "decimal(4,2)")]
        public decimal FactorMultiplicador { get; set; } = 1.00m;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        [Column("actualizado_en")]
        public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual ICollection<TemporadaHabitacionPrecio> HabitacionPrecios { get; set; } = new List<TemporadaHabitacionPrecio>();
    }
}