using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Hotel.Models
{
    [Table("tipos_habitacion")]
    public class TipoHabitacion
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

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        [Column("factor_tipo", TypeName = "decimal(5,2)")]
        public decimal FactorTipo { get; set; } = 1.00m;


        // Navegación
        public virtual ICollection<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();
    }
}