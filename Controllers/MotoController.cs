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

        // POST: api/motos
        [HttpPost]
        public async Task<ActionResult<MotoResponseDto>> CadastrarMoto([FromBody] MotoDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Placa))
                return BadRequest("Placa é obrigatória.");

            var setor = await _context.Setores.FindAsync(dto.SetorId);
            if (setor == null)
                return BadRequest("Setor não encontrado.");

            var moto = new Moto { Placa = dto.Placa, SetorId = setor.Id };
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();

            return Ok(
                new MotoResponseDto
                {
                    Id = moto.Id,
                    Placa = moto.Placa,
                    SetorId = setor.Id,
                    SetorNome = setor.Nome,
                }
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MotoResponseDto>> Update(int id, [FromBody] MotoDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Placa))
                return BadRequest("Placa é obrigatória.");

            var moto = await _context
                .Motos.Include(m => m.Setor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (moto == null)
                return NotFound();

            var setor = await _context.Setores.FindAsync(dto.SetorId);
            if (setor == null)
                return BadRequest("Setor não encontrado.");

            moto.Placa = dto.Placa;
            moto.SetorId = setor.Id;

            await _context.SaveChangesAsync();

            var motoAtualizada = new MotoResponseDto
            {
                Id = moto.Id,
                Placa = moto.Placa,
                SetorId = setor.Id,
                SetorNome = setor.Nome,
            };

            return Ok(motoAtualizada);
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

            return Ok(motos);
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

            return Ok(motos);
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
