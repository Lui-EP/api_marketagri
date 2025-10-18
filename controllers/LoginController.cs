using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMarketApi.data;

namespace AgroMarketApi.controllers
{
    [ApiController]
    [Route("auth")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _db;
        public LoginController(AppDbContext db) { _db = db; }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password && u.Activo);

            if (user == null) return Unauthorized(new { ok = false, msg = "Credenciales inválidas" });

            user.Ultima_Conexion = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { ok = true, id = user.Id, tipo = user.Tipo, nombre = user.Nombre, email = user.Email });
        }

        public class LoginDto
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }
    }
}
