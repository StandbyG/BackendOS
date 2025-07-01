using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSControlSystem.Data;
using OSControlSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSControlSystem.DTOs;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace OSControlSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenServicioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdenServicioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenServicio>>> GetOrdenes()
        {
            return await _context.OrdenesServicio.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenServicio>> GetOrden(int id)
        {
            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null)
                return NotFound();
            return orden;
        }

        [HttpPost]
        public async Task<ActionResult<OrdenServicio>> CreateOrden(OrdenServicio orden)
        {
            _context.OrdenesServicio.Add(orden);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrden), new { id = orden.Id }, orden);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrden(int id, OrdenServicio updated)
        {
            if (id != updated.Id)
                return BadRequest();

            _context.Entry(updated).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrden(int id)
        {
            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null)
                return NotFound();

            _context.OrdenesServicio.Remove(orden);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [Authorize(Roles = "Técnico,Administrador")]
        [HttpPost("{id}/iniciar")]
        public async Task<IActionResult> IniciarOrden(int id, [FromBody] IniciarOrdenDto dto)
        {
            var etapa = dto.Etapa;
            var orden = await _context.OrdenesServicio.FindAsync(id);

            if (orden == null)
                return NotFound("Orden de Servicio no encontrada.");

            if (orden.Estado != "AST aprobado")
                return BadRequest("La OS no puede iniciarse porque no tiene estado 'AST aprobado'.");

            if (etapa != "Desarme" && etapa != "Armado")
                return BadRequest("Etapa inválida. Debe ser 'Desarme' o 'Armado'.");

            // Cambiar estado y etapa actual
            orden.Estado = "IN-PROCESS";
            orden.EtapaActual = etapa;
            orden.FechaInicio = DateTime.Now;

            // Crear nueva etapa
            var nuevaEtapa = new Etapa
            {
                OrdenServicioId = orden.Id,
                Tipo = etapa,
                FechaInicio = DateTime.Now
            };

            _context.Etapas.Add(nuevaEtapa);
            await _context.SaveChangesAsync();

            return Ok("Orden iniciada correctamente.");
        }


        [HttpPost("{id}/parar")]
        public async Task<IActionResult> PararOrden(int id, [FromBody] ParadaRequestDto dto)
        {
            var orden = await _context.OrdenesServicio
                .Include(o => o.Etapas)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null)
                return NotFound("Orden de Servicio no encontrada.");

            if (orden.Estado != "IN-PROCESS")
                return BadRequest("La OS no está en ejecución.");

            var etapa = orden.Etapas
                .OrderByDescending(e => e.FechaInicio)
                .FirstOrDefault();

            if (etapa == null)
                return BadRequest("No se encontró una etapa activa para esta OS.");

            var paradaAbierta = await _context.Paradas
                .AnyAsync(p => p.EtapaId == etapa.Id && p.Fin == null);

            if (paradaAbierta)
                return BadRequest("Ya existe una parada activa.");

            var motivo = await _context.MotivosParada.FindAsync(dto.MotivoId);
            if (motivo == null || motivo.Tipo != dto.Tipo)
                return BadRequest("Motivo de parada inválido o tipo no coincide.");

            var parada = new Parada
            {
                EtapaId = etapa.Id,
                Tipo = dto.Tipo,
                MotivoId = dto.MotivoId,
                Inicio = DateTime.Now
            };

            _context.Paradas.Add(parada);
            await _context.SaveChangesAsync();

            // Placeholder de notificación
            Console.WriteLine($"📧 Notificación: Parada '{dto.Tipo}' registrada.");

            return Ok("Parada registrada correctamente.");
        }


        [HttpPost("{id}/reanudar")]
        public async Task<IActionResult> ReanudarOrden(int id)
        {
            var orden = await _context.OrdenesServicio
                .Include(o => o.Etapas)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null)
                return NotFound("Orden de Servicio no encontrada.");

            if (orden.Estado != "IN-PROCESS")
                return BadRequest("La OS no está en ejecución.");

            var etapa = orden.Etapas
                .OrderByDescending(e => e.FechaInicio)
                .FirstOrDefault();

            if (etapa == null)
                return BadRequest("No se encontró una etapa activa para esta OS.");

            var parada = await _context.Paradas
                .Where(p => p.EtapaId == etapa.Id && p.Fin == null)
                .OrderByDescending(p => p.Inicio)
                .FirstOrDefaultAsync();

            if (parada == null)
                return BadRequest("No hay ninguna parada activa para reanudar.");

            parada.Fin = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok("Parada cerrada y trabajo reanudado correctamente.");
        }

        [HttpPost("{id}/reprogramar")]
        public async Task<IActionResult> ReprogramarOrden(int id, [FromBody] ReprogramacionRequestDto dto)
        {
            var orden = await _context.OrdenesServicio
                .Include(o => o.Etapas)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null)
                return NotFound("Orden de Servicio no encontrada.");

            if (orden.Estado != "IN-PROCESS")
                return BadRequest("La OS no está en ejecución.");

            var etapa = orden.Etapas
                .OrderByDescending(e => e.FechaInicio)
                .FirstOrDefault();

            if (etapa == null)
                return BadRequest("No se encontró una etapa activa.");

            var reprogramacion = new Reprogramacion
            {
                EtapaId = etapa.Id,
                Motivo = dto.Motivo,
                Fecha = DateTime.Now,
                Notificado = true // asumimos que notifica siempre
            };
            
            orden.Estado = "AST aprobado";
            _context.Reprogramaciones.Add(reprogramacion);
            await _context.SaveChangesAsync();

            Console.WriteLine($"📧 Reprogramación registrada: {dto.Motivo}");

            return Ok("Orden reprogramada correctamente.");
        }

        [HttpPost("{id}/finalizar")]
        public async Task<IActionResult> FinalizarOrden(int id)
        {
            var orden = await _context.OrdenesServicio
                .Include(o => o.Etapas)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null)
                return NotFound("Orden de Servicio no encontrada.");

            if (orden.Estado != "IN-PROCESS")
                return BadRequest("La OS no está en ejecución.");

            var etapa = orden.Etapas
                .OrderByDescending(e => e.FechaInicio)
                .FirstOrDefault();

            if (etapa == null)
                return BadRequest("No se encontró una etapa activa.");

            // Cerrar etapa
            etapa.FechaFin = DateTime.Now;

            // Cerrar OS
            orden.Estado = "DONE";
            orden.FechaFin = DateTime.Now;

            // Cerrar cualquier parada abierta (opcional pero importante)
            var paradaAbierta = await _context.Paradas
                .Where(p => p.EtapaId == etapa.Id && p.Fin == null)
                .FirstOrDefaultAsync();

            if (paradaAbierta != null)
            {
                paradaAbierta.Fin = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok("Orden finalizada correctamente.");
        }
        // TO-DO: OS con estado 'AST aprobado'
        [HttpGet("todo")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrdenServicio>>> GetToDo()
        {
            return await _context.OrdenesServicio
                .Where(o => o.Estado == "AST aprobado")
                .ToListAsync();
        }

        // IN-PROCESS: OS en ejecución
        [HttpGet("in-process")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrdenServicio>>> GetInProcess()
        {
            return await _context.OrdenesServicio
                .Where(o => o.Estado == "IN-PROCESS")
                .ToListAsync();
        }

        // DONE: OS finalizadas
        [HttpGet("done")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrdenServicio>>> GetDone()
        {
            return await _context.OrdenesServicio
                .Where(o => o.Estado == "DONE")
                .ToListAsync();
        }

        [HttpGet("{id}/exportar-csv")]
        [Authorize]
        public async Task<IActionResult> ExportarCsv(int id)
        {
            var orden = await _context.OrdenesServicio
                .Include(o => o.Etapas)
                    .ThenInclude(e => e.Paradas)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null)
                return NotFound("Orden no encontrada.");

            // Cargar Reprogramaciones por separado (para evitar conflictos con ThenInclude)
            await _context.Entry(orden)
                .Collection(o => o.Etapas)
                .Query()
                .Include(e => e.Reprogramaciones)
                .LoadAsync();

            var rows = new List<ExportRow>();

            foreach (var etapa in orden.Etapas ?? new List<Etapa>())
            {
                // Paradas
                foreach (var parada in etapa.Paradas ?? new List<Parada>())
                {
                    var motivo = await _context.MotivosParada
                        .Where(m => m.Id == parada.MotivoId)
                        .Select(m => m.Descripcion)
                        .FirstOrDefaultAsync();

                    rows.Add(new ExportRow
                    {
                        CodigoOS = orden.Codigo,
                        EstadoOS = orden.Estado,
                        Etapa = etapa.Tipo,
                        TipoRegistro = "Parada",
                        TipoParada = parada.Tipo,
                        Motivo = motivo ?? "Desconocido",
                        FechaInicio = parada.Inicio,
                        FechaFin = parada.Fin
                    });
                }

                // Reprogramaciones
                foreach (var repro in etapa.Reprogramaciones ?? new List<Reprogramacion>())
                {
                    rows.Add(new ExportRow
                    {
                        CodigoOS = orden.Codigo,
                        EstadoOS = orden.Estado,
                        Etapa = etapa.Tipo,
                        TipoRegistro = "Reprogramación",
                        TipoParada = null,
                        Motivo = repro.Motivo,
                        FechaInicio = repro.Fecha,
                        FechaFin = null
                    });
                }
            }

            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(rows);
            writer.Flush();

            return File(ms.ToArray(), "text/csv", $"OS_{orden.Codigo}_export.csv");
        }

    }
}
