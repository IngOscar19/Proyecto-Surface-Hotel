using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Hotel.Models
{
    [Table("pagos")]
    public class Pago
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("reserva_id")]
        public int ReservaId { get; set; }

        [Required]
        [Column("monto", TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("metodo")]
        public string Metodo { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "pendiente";

        [MaxLength(100)]
        [Column("referencia_transaccion")]
        public string? ReferenciaTransaccion { get; set; }

        [Column("procesado_por")]
        public int? ProcesadoPor { get; set; }

        [Column("notas")]
        public string? Notas { get; set; }

        [Column("pagado_en")]
        public DateTime PagadoEn { get; set; } = DateTime.UtcNow;

        [Column("reembolsado_en")]
        public DateTime? ReembolsadoEn { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("ReservaId")]
        public virtual Reserva Reserva { get; set; } = null!;

        [ForeignKey("ProcesadoPor")]
        public virtual Usuario? UsuarioProcesador { get; set; }
    }
}