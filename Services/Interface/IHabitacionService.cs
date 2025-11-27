using Hotel.Models;
using ProjectHotel.DTOs;
using Microsoft.AspNetCore.Http;

namespace Hotel.Services
{
    public interface IHabitacionService
    {
        // Crear habitación con archivos de fotos
        Task<Habitacion> CrearHabitacionAsync(
            Habitacion habitacion, 
            List<int>? serviciosIds, 
            List<IFormFile>? fotos);

        // Actualizar habitación con nuevas fotos (archivos)
        Task<Habitacion?> ActualizarHabitacionAsync(
            int id, 
            ActualizarHabitacionRequest request, 
            List<IFormFile>? nuevasFotos = null);

        // Agregar foto individual (archivo)
        Task<FotoResponse> AgregarFotoAsync(
            int habitacionId, 
            IFormFile foto, 
            string? descripcion = null, 
            bool esPrincipal = false);

        Task<bool> EliminarFotoAsync(int fotoId);
        Task<HabitacionDetalleResponse?> ObtenerDetalleAsync(int id);
        Task<List<HabitacionDetalleResponse>> ObtenerTodasConDetalleAsync();
        Task<HabitacionDetalleResponse?> ObtenerPorNumeroAsync(string numeroHabitacion);
        Task<List<HabitacionDetalleResponse>> ObtenerDisponiblesAsync();
        Task<List<HabitacionDetalleResponse>> FiltrarHabitacionesAsync(FiltroHabitacionesRequest filtro);
        Task<bool> CambiarEstadoAsync(int id, string estado);
        Task<bool> EliminarHabitacionAsync(int id);
        Task AgregarServicioAsync(int habitacionId, int servicioId);
        Task<bool> EliminarServicioAsync(int habitacionId, int servicioId);
    }
}