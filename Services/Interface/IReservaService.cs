using ProjectHotel.DTOs;

namespace Hotel.Services
{
    public interface IReservaService
    {
        Task<ReservaResponseDto> CrearReservaAsync(ReservaCreateDto dto, int usuarioId);
        Task<ReservaResponseDto?> ObtenerReservaPorIdAsync(int id);
        Task<List<ReservaResponseDto>> ObtenerReservasAsync();
        Task<bool> CancelarReservaAsync(int reservaId, int usuarioId);
    }
}