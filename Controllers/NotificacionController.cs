using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSControlSystem.Data;
using OSControlSystem.Models;

namespace OSControlSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificacionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notificacion>>> GetAll()
        {
            return await _context.Notificaciones
                .Include(n => n.MotivoParada)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notificacion>> Get(int id)
        {
            var notificacion = await _context.Notificaciones
                .Include(n => n.MotivoParada)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notificacion == null) return NotFound();
            return notificacion;
        }

        [HttpPost]
        public async Task<ActionResult<Notificacion>> Create(Notificacion notificacion)
        {
            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = notificacion.Id }, notificacion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Notificacion notificacion)
        {
            if (id != notificacion.Id) return BadRequest();
            _context.Entry(notificacion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null) return NotFound();

            _context.Notificaciones.Remove(notificacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
