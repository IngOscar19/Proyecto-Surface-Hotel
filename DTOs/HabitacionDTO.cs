using Microsoft.AspNetCore.Http;

namespace ProjectHotel.DTOs
{
    // Request para crear habitación (JSON - mantener para compatibilidad)
    public class CrearHabitacionRequest
    {
        public string NumeroHabitacion { get; set; } = string.Empty;
        public int TipoHabitacionId { get; set; }
        public short Piso { get; set; }
        public decimal PrecioBase { get; set; }
        public short Capacidad { get; set; }
        public string? Descripcion { get; set; }
        public List<int>? ServiciosIds { get; set; }
        public List<FotoRequest>? Fotos { get; set; }  
    }

    // NUEVO: Request para crear habitación con FormData
    public class CrearHabitacionFormRequest
    {
        public string NumeroHabitacion { get; set; } = string.Empty;
        public int TipoHabitacionId { get; set; }
        public short Piso { get; set; }
        public decimal PrecioBase { get; set; }
        public short Capacidad { get; set; }
        public string? Descripcion { get; set; }
        public List<int>? ServiciosIds { get; set; }
        public List<IFormFile>? Fotos { get; set; }  // Archivos reales
    }

    public class FotoRequest
    {
        public string Url { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool EsPrincipal { get; set; }
    }

    // Request para actualizar habitación (JSON - mantener para compatibilidad)
    public class ActualizarHabitacionRequest
    {
        public string? NumeroHabitacion { get; set; }
        public int? TipoHabitacionId { get; set; }
        public short? Piso { get; set; }
        public decimal? PrecioBase { get; set; }
        public short? Capacidad { get; set; }
        public string? Descripcion { get; set; }
        public string? Estado { get; set; }
        public List<int>? ServiciosIds { get; set; }
        public List<FotoRequest>? Fotos { get; set; }  
        public bool? ReemplazarFotos { get; set; }  
    }

    // NUEVO: Request para actualizar habitación con FormData
    public class ActualizarHabitacionFormRequest
    {
        public string? NumeroHabitacion { get; set; }
        public int? TipoHabitacionId { get; set; }
        public short? Piso { get; set; }
        public decimal? PrecioBase { get; set; }
        public short? Capacidad { get; set; }
        public string? Descripcion { get; set; }
        public string? Estado { get; set; }
        public List<int>? ServiciosIds { get; set; }
        public List<IFormFile>? NuevasFotos { get; set; }  
        public bool? ReemplazarFotos { get; set; }  
    }

    // Request para filtrar habitaciones
    public class FiltroHabitacionesRequest
    {
        public string? Estado { get; set; }
        public int? TipoHabitacionId { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public short? CapacidadMinima { get; set; }
        public short? Piso { get; set; }
        public List<int>? ServiciosIds { get; set; }
    }

  
    public class CambiarEstadoRequest
    {
        public string Estado { get; set; } = string.Empty;
    }

    
    public class AgregarFotoRequest
    {
        public string Url { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool EsPrincipal { get; set; }
    }

   
    public class HabitacionDetalleResponse
    {
        public int Id { get; set; }
        public string NumeroHabitacion { get; set; } = string.Empty;
        public short Piso { get; set; }
        public decimal PrecioBase { get; set; }
        public short Capacidad { get; set; }
        public string? Descripcion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int TipoHabitacionId { get; set; }
        public TipoHabitacionResponse? TipoHabitacion { get; set; }
        public List<FotoResponse> Fotos { get; set; } = new();
        public List<ServicioResponse> Servicios { get; set; } = new();
        public DateTime CreadoEn { get; set; }
        public DateTime ActualizadoEn { get; set; }
    }

    // Response simple de habitación
    public class HabitacionSimpleResponse
    {
        public int Id { get; set; }
        public string NumeroHabitacion { get; set; } = string.Empty;
        public short Piso { get; set; }
        public decimal PrecioBase { get; set; }
        public short Capacidad { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? TipoHabitacion { get; set; }
    }

    // DTOs auxiliares
    public class FotoResponse
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool EsPrincipal { get; set; }
    }

    public class TipoHabitacionResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

   
}