using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Hotel.Models
{
    [Table("temporada_habitacion_precios")]
    public class TemporadaHabitacionPrecio
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("temporada_id")]
        public int TemporadaId { get; set; }

        [Column("habitacion_id")]
        public int HabitacionId { get; set; }

        [Required]
        [Column("precio_override", TypeName = "decimal(10,2)")]
        public decimal PrecioOverride { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("TemporadaId")]
        public virtual TemporadaPrecio Temporada { get; set; } = null!;

        [ForeignKey("HabitacionId")]
        public virtual Habitacion Habitacion { get; set; } = null!;
    }
}