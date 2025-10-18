using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMarketApi.data;
using AgroMarketApi.models;

namespace AgroMarketApi.controllers
{
    [ApiController]
    [Route("usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsuariosController(AppDbContext db) { _db = db; }
// GET /usuarios  → lista todos (solo para pruebas o admin)
[HttpGet]
public async Task<IActionResult> GetAll()
{
    var usuarios = await _db.Usuarios
        .OrderBy(u => u.Id)
        .Select(u => new
        {
            u.Id,
            u.Tipo,
            u.Nombre,
            u.Email,
            u.Ubicacion,
            u.Telefono,
            u.Activo,
            u.Fecha_Registro,
            u.Ultima_Conexion
        })
        .ToListAsync();

    return Ok(usuarios);
}

        // POST /usuarios  → registrar nuevo usuario
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] UsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Tipo) ||
                string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { ok = false, msg = "Faltan campos obligatorios" });

            var existe = await _db.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (existe) return Conflict(new { ok = false, msg = "El correo ya está registrado" });

            var user = new Usuario
            {
                Tipo = dto.Tipo,
                Nombre = dto.Nombre,
                Email = dto.Email,
                Password = dto.Password,
                Ubicacion = dto.Ubicacion,
                Telefono = dto.Telefono,
                Activo = true,
                Fecha_Registro = DateTime.UtcNow
            };

            _db.Usuarios.Add(user);
            await _db.SaveChangesAsync();

            return Created($"/usuarios/{user.Id}",
                new { ok = true, id = user.Id, tipo = user.Tipo, nombre = user.Nombre, email = user.Email });
        }

        // GET /usuarios/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var u = await _db.Usuarios.FindAsync(id);
            if (u == null || !u.Activo)
                return NotFound(new { ok = false, msg = "Usuario no encontrado" });

            return Ok(new
            {
                u.Id,
                u.Tipo,
                u.Nombre,
                u.Email,
                u.Ubicacion,
                u.Telefono,
                u.Fecha_Registro,
                u.Ultima_Conexion
            });
        }

        // PUT /usuarios/{id}  → actualizar perfil
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UsuarioDto dto)
        {
            var u = await _db.Usuarios.FindAsync(id);
            if (u == null) return NotFound(new { ok = false, msg = "Usuario no encontrado" });

            if (!string.IsNullOrWhiteSpace(dto.Nombre)) u.Nombre = dto.Nombre;
            if (!string.IsNullOrWhiteSpace(dto.Ubicacion)) u.Ubicacion = dto.Ubicacion;
            if (!string.IsNullOrWhiteSpace(dto.Telefono)) u.Telefono = dto.Telefono;
            if (!string.IsNullOrWhiteSpace(dto.Password)) u.Password = dto.Password;

            await _db.SaveChangesAsync();
            return Ok(new { ok = true, msg = "Usuario actualizado" });
        }

        // DELETE /usuarios/{id}?hard=false
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Borrar(int id, [FromQuery] bool hard = false)
        {
            var u = await _db.Usuarios.FindAsync(id);
            if (u == null) return NotFound(new { ok = false, msg = "Usuario no encontrado" });

            if (hard)
                _db.Usuarios.Remove(u);
            else
                u.Activo = false;

            await _db.SaveChangesAsync();
            return Ok(new { ok = true, msg = "Usuario eliminado" });
        }

        // DTO simple (solo campos del formulario)
        public class UsuarioDto
        {
            public string Tipo { get; set; } = "";
            public string Nombre { get; set; } = "";
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
            public string? Ubicacion { get; set; }
            public string? Telefono { get; set; }
        }
    }
}
