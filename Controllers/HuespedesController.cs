using Microsoft.AspNetCore.Mvc;
using Hotel.Services;
using ProjectHotel.DTOs;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HuespedController : ControllerBase
    {
        private readonly IHuespedService _service;

        public HuespedController(IHuespedService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearHuespedDTO dto)
        {
            var huesped = await _service.Crear(dto);
            return Ok(huesped);
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var lista = await _service.Listar();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var h = await _service.Obtener(id);
            if (h == null) return NotFound();
            return Ok(h);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] CrearHuespedDTO dto)
        {
            var actualizado = await _service.Actualizar(id, dto);
            if (actualizado == null) return NotFound();
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.Eliminar(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}
