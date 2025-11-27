using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models
{
    [Table("habitacion_fotos")]
    public class HabitacionFoto
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("habitacion_id")]
        public int HabitacionId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("url")]
        public string Url { get; set; } = string.Empty; 

        [MaxLength(255)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("es_principal")]
        public bool EsPrincipal { get; set; } = false;

        [Required]
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("HabitacionId")]
        public virtual Habitacion Habitacion { get; set; } = null!;
    }
}