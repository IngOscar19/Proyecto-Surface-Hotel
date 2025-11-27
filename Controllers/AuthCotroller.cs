using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel.Services;
using Hotel.Models;

namespace Hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        // Inyectamos solo el UsuarioService
        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // DTO para recibir los datos del login
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        [AllowAnonymous] 
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "El email y la contraseña son obligatorios." });
            }

            try
            {
               
                var emailLimpio = request.Email.Trim();
                var passwordLimpio = request.Password.Trim();

                
                var token = await _usuarioService.LoginAsync(emailLimpio, passwordLimpio);

                // 4. Éxito
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("perfil")]
       
        public IActionResult Perfil()
        {
            return Ok(new
            {
                message = "Acceso concedido",
                user = User.Identity?.Name,
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }
}