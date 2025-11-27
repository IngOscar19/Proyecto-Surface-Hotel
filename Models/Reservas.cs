using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models
{
    [Table("reservas")]
    public class Reserva
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("habitacion_id")]
        public int HabitacionId { get; set; }

        [Column("huesped_id")]
        public int HuespedId { get; set; }

        [Required]
        [Column("fecha_entrada")]
        public DateTime FechaEntrada { get; set; }

        [Required]
        [Column("fecha_salida")]
        public DateTime FechaSalida { get; set; }

        [Column("numero_noches")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int NumeroNoches { get; private set; }  // ← CAMBIADO A int

        [Column("numero_huespedes")]
        public int NumeroHuespedes { get; set; } = 1;  // ← CAMBIADO A int

        [Required]
        [MaxLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "pendiente";

        [Required]
        [Column("precio_por_noche", TypeName = "decimal(10,2)")]
        public decimal PrecioPorNoche { get; set; }

        [Required]
        [Column("precio_total", TypeName = "decimal(10,2)")]
        public decimal PrecioTotal { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("creado_por")]
        public int CreadoPor { get; set; }

        [Column("cancelado_por")]
        public int? CanceladoPor { get; set; }

        [Column("fecha_cancelacion")]
        public DateTime? FechaCancelacion { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        [Column("actualizado_en")]
        public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("HabitacionId")]
        public virtual Habitacion Habitacion { get; set; } = null!;

        [ForeignKey("HuespedId")]
        public virtual Huesped Huesped { get; set; } = null!;

        [ForeignKey("CreadoPor")]
        public virtual Usuario UsuarioCreador { get; set; } = null!;

        [ForeignKey("CanceladoPor")]
        public virtual Usuario? UsuarioCancelador { get; set; }

        public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}