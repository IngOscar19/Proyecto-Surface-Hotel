namespace ProjectHotel.DTOs
{
    public class ReservaCreateDto
    {
        public int HabitacionId { get; set; }
        public int HuespedId { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public int NumeroHuespedes { get; set; } = 1;
        public string? Observaciones { get; set; }
    }

    public class ReservaResponseDto
    {
        public int Id { get; set; }
        public int HabitacionId { get; set; }
        public int HuespedId { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public int NumeroNoches { get; set; }
        public int NumeroHuespedes { get; set; }
        public string Estado { get; set; } = null!;
        public decimal PrecioPorNoche { get; set; }
        public decimal PrecioTotal { get; set; }
        public string? Observaciones { get; set; }
        public DateTime CreadoEn { get; set; }
        
        // Información relacionada (opcional)
        public string? NombreHabitacion { get; set; }
        public string? NombreHuesped { get; set; }
    }
}