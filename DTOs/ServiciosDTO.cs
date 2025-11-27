namespace ProjectHotel.DTOs
{
    public class CrearServicioRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
    }

    public class ActualizarServicioRequest
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
        public bool? Activo { get; set; }
    }

    public class ServicioResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
        public bool Activo { get; set; }
        
    }
}