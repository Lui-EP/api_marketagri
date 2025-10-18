using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMarketApi.data;
using AgroMarketApi.models;

namespace AgroMarketApi.controllers
{
    [ApiController]
    [Route("chats")]
    public class ChatsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ChatsController(AppDbContext db) { _db = db; }

        // GET /chats/de-usuario/{usuarioId}
        [HttpGet("de-usuario/{usuarioId:int}")]
        public async Task<IActionResult> MisChats(int usuarioId)
        {
            var chats = await _db.Chats
                .Where(c => c.Productor_Id == usuarioId || c.Empresa_Id == usuarioId)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
            return Ok(chats);
        }

        // GET /chats/{chatId}/mensajes
        [HttpGet("{chatId:int}/mensajes")]
        public async Task<IActionResult> Mensajes(int chatId)
        {
            var msgs = await _db.Mensajes
                .Where(m => m.Chat_Id == chatId)
                .OrderBy(m => m.Id)
                .ToListAsync();
            return Ok(msgs);
        }

        public class EnviarDto
        {
            public int Remitente_Id { get; set; }
            public string Mensaje { get; set; } = "";
        }

        // POST /chats/{chatId}/mensajes
        [HttpPost("{chatId:int}/mensajes")]
        public async Task<IActionResult> Enviar(int chatId, [FromBody] EnviarDto dto)
        {
            var existeChat = await _db.Chats.AnyAsync(c => c.Id == chatId);
            if (!existeChat) return NotFound(new { ok = false, msg = "Chat no existe" });

            var msg = new Mensaje
            {
                Chat_Id = chatId,
                Remitente_Id = dto.Remitente_Id,
                Mensaje_Texto = dto.Mensaje,
                Fecha_Envio = DateTime.UtcNow,
                Leido = false
            };
            _db.Mensajes.Add(msg);
            await _db.SaveChangesAsync();
            return Ok(msg);
        }

        // PUT /chats/{chatId}
        [HttpPut("{chatId:int}")]
        public async Task<IActionResult> ActualizarChat(int chatId, [FromBody] Chat dto)
        {
            var c = await _db.Chats.FindAsync(chatId);
            if (c == null) return NotFound(new { ok = false, msg = "Chat no existe" });

            // (no tenemos campos editables realmente, se deja así para futuro)
            await _db.SaveChangesAsync();
            return Ok(new { ok = true, c });
        }

        // DELETE /chats/{chatId}
        [HttpDelete("{chatId:int}")]
        public async Task<IActionResult> BorrarChat(int chatId)
        {
            var c = await _db.Chats.FindAsync(chatId);
            if (c == null) return NotFound(new { ok = false, msg = "Chat no existe" });

            _db.Chats.Remove(c); // cascada → borra mensajes del chat
            await _db.SaveChangesAsync();
            return Ok(new { ok = true });
        }

        // PUT /chats/{chatId}/mensajes/{mensajeId}
        [HttpPut("{chatId:int}/mensajes/{mensajeId:int}")]
        public async Task<IActionResult> EditarMensaje(int chatId, int mensajeId, [FromBody] EnviarDto dto)
        {
            var m = await _db.Mensajes.FirstOrDefaultAsync(x => x.Id == mensajeId && x.Chat_Id == chatId);
            if (m == null) return NotFound(new { ok = false, msg = "Mensaje no existe" });

            if (!string.IsNullOrWhiteSpace(dto.Mensaje)) m.Mensaje_Texto = dto.Mensaje;
            await _db.SaveChangesAsync();
            return Ok(new { ok = true, m });
        }

        // DELETE /chats/{chatId}/mensajes/{mensajeId}
        [HttpDelete("{chatId:int}/mensajes/{mensajeId:int}")]
        public async Task<IActionResult> BorrarMensaje(int chatId, int mensajeId)
        {
            var m = await _db.Mensajes.FirstOrDefaultAsync(x => x.Id == mensajeId && x.Chat_Id == chatId);
            if (m == null) return NotFound(new { ok = false, msg = "Mensaje no existe" });

            _db.Mensajes.Remove(m);
            await _db.SaveChangesAsync();
            return Ok(new { ok = true });
        }
    }
}
