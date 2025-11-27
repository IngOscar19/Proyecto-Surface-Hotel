using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models
{
    [Table("habitacion_servicio")]
    public class HabitacionServicio
    {
        [Column("habitacion_id")]
        public int HabitacionId { get; set; }

        [Column("servicio_id")]
        public int ServicioId { get; set; }

        // Navegación
        public Habitacion Habitacion { get; set; }
        public Servicio Servicio { get; set; }

       
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}
