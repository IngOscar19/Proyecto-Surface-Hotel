using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Models;

namespace Hotel.Services
{
    public interface ITemporadaPrecioService
    {
        Task<IEnumerable<TemporadaPrecio>> ObtenerTodosAsync();
        Task<IEnumerable<TemporadaPrecio>> ObtenerActivosAsync();
        Task<TemporadaPrecio?> ObtenerPorIdAsync(int id);
        Task<TemporadaPrecio?> ObtenerPorFechaAsync(DateTime fecha);
        Task<TemporadaPrecio> CrearAsync(TemporadaPrecio temporada);
        Task<TemporadaPrecio?> ActualizarAsync(int id, TemporadaPrecio temporada);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteTraslapeAsync(DateTime fechaInicio, DateTime fechaFin, int? temporadaIdExcluir = null);
        Task<bool> ActivarDesactivarAsync(int id, bool activo);
    }
}