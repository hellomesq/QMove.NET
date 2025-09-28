using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Data;
using MotoMonitoramento.DTOs;
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
        public async Task<ActionResult<IEnumerable<Setor>>> GetAll()
        {
            return Ok(await _context.Setores.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Setor>> CadastrarSetor([FromBody] SetorDto dto)
        {
            if (string.IsNullOrEmpty(dto.Nome))
                return BadRequest("Nome do setor é obrigatório.");

            var setor = new Setor { Nome = dto.Nome };
            _context.Setores.Add(setor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = setor.Id }, setor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SetorDto dto)
        {
            if (string.IsNullOrEmpty(dto.Nome))
                return BadRequest("Nome do setor é obrigatório.");

            var setor = await _context.Setores.FindAsync(id);
            if (setor == null)
                return NotFound();

            setor.Nome = dto.Nome;
            await _context.SaveChangesAsync();

            return NoContent();
        }

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
