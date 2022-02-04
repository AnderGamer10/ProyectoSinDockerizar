using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiTiempo.Models;

namespace WebApiTiempo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformacionTiempoesController : ControllerBase
    {
        private readonly TiempoContext _context;

        public InformacionTiempoesController(TiempoContext context)
        {
            _context = context;
        }

        // GET: api/InformacionTiempoes
        [Autohorrize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InformacionTiempo>>> GetTiempoItems()
        {
            return await _context.TiempoItems.ToListAsync();
        }

        // GET: api/InformacionTiempoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InformacionTiempo>> GetInformacionTiempo(string id)
        {
            var informacionTiempo = await _context.TiempoItems.FindAsync(id);

            if (informacionTiempo == null)
            {
                return NotFound();
            }

            return informacionTiempo;
        }

        // PUT: api/InformacionTiempoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInformacionTiempo(string id, InformacionTiempo informacionTiempo)
        {
            if (id != informacionTiempo.Id)
            {
                return BadRequest();
            }

            _context.Entry(informacionTiempo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InformacionTiempoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/InformacionTiempoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InformacionTiempo>> PostInformacionTiempo(InformacionTiempo informacionTiempo)
        {
            _context.TiempoItems.Add(informacionTiempo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (InformacionTiempoExists(informacionTiempo.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetInformacionTiempo", new { id = informacionTiempo.Id }, informacionTiempo);
        }

        // DELETE: api/InformacionTiempoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInformacionTiempo(string id)
        {
            var informacionTiempo = await _context.TiempoItems.FindAsync(id);
            if (informacionTiempo == null)
            {
                return NotFound();
            }

            _context.TiempoItems.Remove(informacionTiempo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InformacionTiempoExists(string id)
        {
            return _context.TiempoItems.Any(e => e.Id == id);
        }
    }
}
