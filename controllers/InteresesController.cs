using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMarketApi.data;
using AgroMarketApi.models;

namespace AgroMarketApi.controllers
{
    [ApiController]
    [Route("intereses")]
    public class InteresesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public InteresesController(AppDbContext db) { _db = db; }

        public class CrearInteresDto
        {
            public int Producto_Id { get; set; }
            public int Empresa_Id { get; set; }
            public string? Notas { get; set; }
        }

        // POST /intereses  (botón "Me interesa")
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearInteresDto dto)
        {
            // upsert simple (evita duplicados del mismo buyer)
            var interes = await _db.Intereses
                .FirstOrDefaultAsync(i => i.Producto_Id == dto.Producto_Id && i.Empresa_Id == dto.Empresa_Id);

            if (interes == null)
            {
                interes = new Interes
                {
                    Producto_Id = dto.Producto_Id,
                    Empresa_Id = dto.Empresa_Id,
                    Notas = dto.Notas,
                    Fecha_Interes = DateTime.UtcNow,
                    Activo = true
                };
                _db.Intereses.Add(interes);
                await _db.SaveChangesAsync();
            }
            else
            {
                interes.Activo = true;
                interes.Notas = dto.Notas;
                await _db.SaveChangesAsync();
            }

            // crear chat si no existe
            var prod = await _db.Productos.FirstAsync(p => p.Id == dto.Producto_Id);
            var chat = await _db.Chats
                .FirstOrDefaultAsync(c => c.Producto_Id == dto.Producto_Id && c.Empresa_Id == dto.Empresa_Id);

            if (chat == null)
            {
                chat = new Chat
                {
                    Producto_Id = dto.Producto_Id,
                    Productor_Id = prod.Productor_Id,
                    Empresa_Id = dto.Empresa_Id,
                    Fecha_Creacion = DateTime.UtcNow
                };
                _db.Chats.Add(chat);
                await _db.SaveChangesAsync();
            }

            return Ok(new { ok = true, interesId = interes.Id, chatId = chat.Id });
        }

        // GET /intereses/por-productor/{productorId}
        [HttpGet("por-productor/{productorId:int}")]
        public async Task<IActionResult> PorProductor(int productorId)
        {
            var data = await (from i in _db.Intereses
                              join p in _db.Productos on i.Producto_Id equals p.Id
                              join e in _db.Usuarios on i.Empresa_Id equals e.Id
                              where p.Productor_Id == productorId && i.Activo
                              orderby i.Id descending
                              select new
                              {
                                  interesId = i.Id,
                                  productoId = p.Id,
                                  producto = p.Nombre,
                                  empresa = e.Nombre,
                                  fecha = i.Fecha_Interes,
                                  notas = i.Notas
                              }).ToListAsync();

            return Ok(data);
        }

        // GET /intereses/por-empresa/{empresaId}
        [HttpGet("por-empresa/{empresaId:int}")]
        public async Task<IActionResult> PorEmpresa(int empresaId)
        {
            var data = await (from i in _db.Intereses
                              join p in _db.Productos on i.Producto_Id equals p.Id
                              where i.Empresa_Id == empresaId && i.Activo
                              orderby i.Id descending
                              select new
                              {
                                  interesId = i.Id,
                                  productoId = p.Id,
                                  producto = p.Nombre,
                                  fecha = i.Fecha_Interes,
                                  notas = i.Notas
                              }).ToListAsync();

            return Ok(data);
        }

        // PUT /intereses/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Interes dto)
        {
            var i = await _db.Intereses.FindAsync(id);
            if (i == null) return NotFound(new { ok = false, msg = "Interés no existe" });

            if (dto.Notas != null) i.Notas = dto.Notas;
            i.Activo = dto.Activo;

            await _db.SaveChangesAsync();
            return Ok(new { ok = true, i });
        }

        // DELETE /intereses/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Borrar(int id)
        {
            var i = await _db.Intereses.FindAsync(id);
            if (i == null) return NotFound(new { ok = false, msg = "Interés no existe" });

            _db.Intereses.Remove(i);   // NO borra chat (chat es por producto-empresa)
            await _db.SaveChangesAsync();
            return Ok(new { ok = true });
        }
    }
}
