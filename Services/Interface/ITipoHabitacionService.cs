using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Models;

namespace Hotel.Services
{
    public interface ITipoHabitacionService
    {
        Task<IEnumerable<TipoHabitacion>> ObtenerTodosAsync();
        Task<TipoHabitacion?> ObtenerPorIdAsync(int id);
        Task<TipoHabitacion> CrearAsync(TipoHabitacion tipoHabitacion);
        Task<TipoHabitacion?> ActualizarAsync(int id, TipoHabitacion tipoHabitacion);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null);
    }
}