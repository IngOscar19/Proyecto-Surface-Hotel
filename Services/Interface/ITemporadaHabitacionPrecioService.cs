using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Models;

namespace Hotel.Services.Interfaces
{
    public interface ITemporadaHabitacionPrecioService
    {
        Task<IEnumerable<TemporadaHabitacionPrecio>> ObtenerTodosAsync();
        Task<IEnumerable<TemporadaHabitacionPrecio>> ObtenerPorTemporadaAsync(int temporadaId);
        Task<IEnumerable<TemporadaHabitacionPrecio>> ObtenerPorHabitacionAsync(int habitacionId);
        Task<TemporadaHabitacionPrecio?> ObtenerPorIdAsync(int id);
        Task<TemporadaHabitacionPrecio?> ObtenerPorTemporadaYHabitacionAsync(int temporadaId, int habitacionId);
        Task<decimal> ObtenerPrecioHabitacionAsync(int habitacionId, DateTime fecha);
        Task<TemporadaHabitacionPrecio> CrearAsync(TemporadaHabitacionPrecio temporadaHabitacionPrecio);
        Task<IEnumerable<TemporadaHabitacionPrecio>> CrearMultiplesAsync(int temporadaId, List<TemporadaHabitacionPrecio> precios);
        Task<TemporadaHabitacionPrecio?> ActualizarAsync(int id, TemporadaHabitacionPrecio temporadaHabitacionPrecio);
        Task<bool> EliminarAsync(int id);
        Task<bool> EliminarPorTemporadaAsync(int temporadaId);
        Task<bool> ExistePrecioAsync(int temporadaId, int habitacionId, int? idExcluir = null);
    }
}