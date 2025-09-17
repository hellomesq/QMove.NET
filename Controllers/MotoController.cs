using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Data;
using MotoMonitoramento.Dtos;
using MotoMonitoramento.Models;

namespace MotoMonitoramento.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MotosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/motos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Moto>>> GetAll() =>
            await _context.Motos.ToListAsync();

        // GET: api/motos/por-setor?setorId=1
        [HttpGet("por-setor")]
        public async Task<ActionResult<IEnumerable<Moto>>> GetPorSetor([FromQuery] int setorId)
        {
            var motos = await _context
                .Motos.Include(m => m.Setor) // inclui dados do setor
                .Where(m => m.SetorId == setorId)
                .ToListAsync();

            return motos;
        }

        // GET: api/motos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Moto>> GetById(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            return moto == null ? NotFound() : Ok(moto);
        }

        // POST: api/motos
        [HttpPost]
        public async Task<ActionResult<Moto>> Create([FromBody] MotoDto dto)
        {
            // Sempre começa no setor "Disponível"
            var setorDisponivel = await _context.Setores.FirstOrDefaultAsync(s =>
                s.Nome == "Disponível"
            );

            if (setorDisponivel == null)
                return BadRequest("Setor 'Disponível' não encontrado. Cadastre-o primeiro.");

            var moto = new Moto
            {
                Placa = dto.Placa,
                Status = dto.Status,
                SetorId = setorDisponivel.Id,
            };

            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = moto.Id }, moto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] MotoDto dto,
            [FromQuery] int? setorId
        )
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                return NotFound();

            moto.Placa = dto.Placa;
            moto.Status = dto.Status;

            if (setorId.HasValue)
            {
                var setor = await _context.Setores.FindAsync(setorId.Value);
                if (setor == null)
                    return BadRequest("Setor informado não existe.");
                moto.SetorId = setor.Id;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/motos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                return NotFound();

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MotoExists(int id) => _context.Motos.Any(m => m.Id == id);
    }
}
