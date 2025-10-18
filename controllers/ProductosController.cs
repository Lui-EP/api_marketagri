using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMarketApi.data;
using AgroMarketApi.models;

namespace AgroMarketApi.controllers
{
    [ApiController]
    [Route("productos")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ProductosController(AppDbContext db) { _db = db; }

        // GET /productos?q=&ubicacion=&minPrecio=&maxPrecio=
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? q,
            [FromQuery] string? ubicacion,
            [FromQuery] decimal? minPrecio,
            [FromQuery] decimal? maxPrecio)
        {
            var query = _db.Productos.Where(p => p.Activo);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(p => p.Nombre.ToLower().Contains(q.ToLower())
                    || (p.Descripcion ?? "").ToLower().Contains(q.ToLower()));

            if (!string.IsNullOrWhiteSpace(ubicacion))
                query = query.Where(p => (p.Ubicacion ?? "").ToLower().Contains(ubicacion.ToLower()));

            if (minPrecio.HasValue) query = query.Where(p => p.Precio >= minPrecio.Value);
            if (maxPrecio.HasValue) query = query.Where(p => p.Precio <= maxPrecio.Value);

            var list = await query.OrderByDescending(p => p.Id).ToListAsync();
            return Ok(list);
        }

        // GET /productos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var p = await _db.Productos.FindAsync(id);
            if (p == null) return NotFound(new { ok = false, msg = "Producto no existe" });
            return Ok(p);
        }

        // POST /productos
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Producto p)
        {
            p.Fecha_Publicacion = DateTime.UtcNow;
            p.Activo = true;
            _db.Productos.Add(p);
            await _db.SaveChangesAsync();
            return Ok(p);
        }

        // PUT /productos/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Producto dto)
        {
            var p = await _db.Productos.FindAsync(id);
            if (p == null) return NotFound(new { ok = false, msg = "Producto no existe" });

            if (!string.IsNullOrWhiteSpace(dto.Nombre)) p.Nombre = dto.Nombre;
            if (!string.IsNullOrWhiteSpace(dto.Ubicacion)) p.Ubicacion = dto.Ubicacion;
            if (!string.IsNullOrWhiteSpace(dto.Descripcion)) p.Descripcion = dto.Descripcion;
            if (dto.Precio > 0) p.Precio = dto.Precio;
            if (dto.Volumen > 0) p.Volumen = dto.Volumen;

            await _db.SaveChangesAsync();
            return Ok(new { ok = true, p });
        }

        // DELETE /productos/{id}?hard=true|false
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Borrar(int id, [FromQuery] bool hard = false)
        {
            var p = await _db.Productos.FindAsync(id);
            if (p == null) return NotFound(new { ok = false, msg = "Producto no existe" });

            if (hard) _db.Productos.Remove(p);          // borra y por cascada se van chats/mensajes
            else p.Activo = false;                      // baja lógica

            await _db.SaveChangesAsync();
            return Ok(new { ok = true });
        }
    }
}
