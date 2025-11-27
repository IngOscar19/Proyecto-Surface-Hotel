using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Hotel.Models
{
    [Table("habitaciones")]
    public class Habitacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("numero_habitacion")]
        public string NumeroHabitacion { get; set; } = string.Empty;

        [Column("tipo_habitacion_id")]
        public int TipoHabitacionId { get; set; }

        [Column("piso")]
        public short? Piso { get; set; }

        [Required]
        [Column("precio_base", TypeName = "decimal(10,2)")]
        public decimal PrecioBase { get; set; }

        [Required]
        [Column("capacidad")]
        public short Capacidad { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "disponible";

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        [Column("actualizado_en")]
        public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("TipoHabitacionId")]
        public virtual TipoHabitacion TipoHabitacion { get; set; } = null!;
        
        public virtual ICollection<HabitacionServicio> HabitacionServicios { get; set; } = new List<HabitacionServicio>();
        public virtual ICollection<HabitacionFoto> Fotos { get; set; } = new List<HabitacionFoto>();

        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public virtual ICollection<TemporadaHabitacionPrecio> TemporadaPrecios { get; set; } = new List<TemporadaHabitacionPrecio>(); 
    }
}