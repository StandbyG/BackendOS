using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSControlSystem.Data;
using OSControlSystem.Models;

namespace OSControlSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermisoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PermisoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Permiso>>> GetAll()
        {
            return await _context.Permisos
                .Include(p => p.Usuario)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Permiso>> Get(int id)
        {
            var permiso = await _context.Permisos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (permiso == null) return NotFound();
            return permiso;
        }

        [HttpPost]
        public async Task<ActionResult<Permiso>> Create(Permiso permiso)
        {
            _context.Permisos.Add(permiso);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = permiso.Id }, permiso);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Permiso permiso)
        {
            if (id != permiso.Id) return BadRequest();
            _context.Entry(permiso).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso == null) return NotFound();

            _context.Permisos.Remove(permiso);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
