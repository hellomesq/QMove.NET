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
        public async Task<ActionResult<IEnumerable<MotoResponseDto>>> GetAll()
        {
            var motos = await _context
                .Motos.Include(m => m.Setor)
                .Select(m => new MotoResponseDto
                {
                    Id = m.Id,
                    Placa = m.Placa,
                    SetorId = m.SetorId,
                    SetorNome = m.Setor != null ? m.Setor.Nome : null,
                })
                .ToListAsync();

            return motos;
        }

        // GET: api/motos/por-setor?setorId=1
        [HttpGet("por-setor")]
        public async Task<ActionResult<IEnumerable<MotoResponseDto>>> GetPorSetor(
            [FromQuery] int setorId
        )
        {
            var motos = await _context
                .Motos.Include(m => m.Setor)
                .Where(m => m.SetorId == setorId)
                .Select(m => new MotoResponseDto
                {
                    Id = m.Id,
                    Placa = m.Placa,
                    SetorId = m.SetorId,
                    SetorNome = m.Setor != null ? m.Setor.Nome : null,
                })
                .ToListAsync();

            return motos;
        }

        // GET: api/motos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MotoResponseDto>> GetById(int id)
        {
            var moto = await _context
                .Motos.Include(m => m.Setor)
                .Where(m => m.Id == id)
                .Select(m => new MotoResponseDto
                {
                    Id = m.Id,
                    Placa = m.Placa,
                    SetorId = m.SetorId,
                    SetorNome = m.Setor != null ? m.Setor.Nome : null,
                })
                .FirstOrDefaultAsync();

            return moto == null ? NotFound() : Ok(moto);
        }

        // POST: api/motos
        [HttpPost("motos")]
        public async Task<ActionResult<MotoDto>> CadastrarMoto(
            [FromQuery] string placa,
            [FromQuery] int setorId
        )
        {
            var setor = await _context.Setores.FindAsync(setorId);
            if (setor == null)
                return BadRequest("Setor não encontrado.");

            var moto = new Moto { Placa = placa, SetorId = setorId };
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();

            return Ok(
                new MotoDto
                {
                    Id = moto.Id,
                    Placa = moto.Placa,
                    SetorNome = setor.Nome,
                }
            );
        }

        // PUT: api/motos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MotoDto dto)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                return NotFound();

            moto.Placa = dto.Placa;

            if (!string.IsNullOrWhiteSpace(dto.SetorNome))
            {
                var setor = await _context.Setores.FirstOrDefaultAsync(s =>
                    EF.Functions.Like(s.Nome, dto.SetorNome.Trim())
                );

                if (setor == null)
                    return BadRequest($"Setor '{dto.SetorNome}' não encontrado.");

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
    }
}
