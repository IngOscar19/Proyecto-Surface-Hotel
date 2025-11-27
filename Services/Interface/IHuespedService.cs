using Hotel.Models;
using ProjectHotel.DTOs;

namespace Hotel.Services
{
    public interface IHuespedService
    {
        Task<Huesped> Crear(CrearHuespedDTO dto);
        Task<List<Huesped>> Listar();
        Task<Huesped?> Obtener(int id);
        Task<Huesped?> Actualizar(int id, CrearHuespedDTO dto);
        Task<bool> Eliminar(int id);
    }
}
