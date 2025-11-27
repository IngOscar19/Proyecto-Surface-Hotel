using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Hotel.Models
{

[Table("huespedes")]
    public class Huesped
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [Column("apellido")]
        public string Apellido { get; set; } = string.Empty;

        [MaxLength(150)]
        [Column("email")]
        public string? Email { get; set; }

        [MaxLength(30)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [MaxLength(50)]
        [Column("numero_documento")]
        public string? NumeroDocumento { get; set; }

        [MaxLength(20)]
        [Column("tipo_documento")]
        public string? TipoDocumento { get; set; }

        [MaxLength(50)]
        [Column("nacionalidad")]
        public string? Nacionalidad { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("fecha_nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        [Column("actualizado_en")]
        public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        
        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}   