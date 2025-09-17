using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Data;
using MotoMonitoramento.Models;

namespace MotoMonitoramento.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SetoresController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Setor>>> GetAll() =>
            await _context.Setores.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Setor>> Create(Setor setor)
        {
            _context.Setores.Add(setor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = setor.Id }, setor);
        }

        // PUT: api/setores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Setor setor)
        {
            if (id != setor.Id)
                return BadRequest();

            _context.Entry(setor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Setores.Any(s => s.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/setores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var setor = await _context.Setores.FindAsync(id);
            if (setor == null)
                return NotFound();

            _context.Setores.Remove(setor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
