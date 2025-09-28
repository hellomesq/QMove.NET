using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Data;
using MotoMonitoramento.Dtos;
using MotoMonitoramento.Models;

namespace MotoMonitoramento.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimentacoesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MovimentacoesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/movimentacoes
        [HttpPost("movimentacoes")]
        public async Task<ActionResult<MovimentacaoDto>> RegistrarMovimentacao(
            [FromQuery] int motoId,
            [FromQuery] int novoSetorId
        )
        {
            var moto = await _context.Motos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == motoId);

            if (moto == null)
                return NotFound("Moto não encontrada.");

            var setorNovo = await _context
                .Setores.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == novoSetorId);

            if (setorNovo == null)
                return BadRequest("Setor novo não encontrado.");

            var movimentacao = new Movimentacao
            {
                MotoId = moto.Id,
                SetorAntigoId = moto.SetorId ?? 0,
                SetorNovoId = setorNovo.Id,
                DataHora = DateTime.UtcNow,
            };

            moto.SetorId = setorNovo.Id;

            _context.Movimentacoes.Add(movimentacao);
            await _context.SaveChangesAsync();

            return Ok(
                new MovimentacaoDto
                {
                    Id = movimentacao.Id,
                    MotoPlaca = moto.Placa,
                    SetorAntigo = (await _context.Setores.FindAsync(moto.SetorId))?.Nome ?? "N/A",
                    SetorNovo = setorNovo.Nome,
                    DataHora = movimentacao.DataHora,
                }
            );
        }

        // GET: api/movimentacoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimentacaoDto>>> ListarTodas()
        {
            var movimentacoes = await _context
                .Movimentacoes.AsNoTracking()
                .OrderByDescending(m => m.DataHora)
                .Select(m => new MovimentacaoDto
                {
                    Id = m.Id,
                    MotoPlaca = m.Moto.Placa,
                    SetorAntigo = m.SetorAntigo.Nome ?? "N/A",
                    SetorNovo = m.SetorNovo.Nome ?? "N/A",
                    DataHora = m.DataHora,
                })
                .ToListAsync();

            return Ok(movimentacoes);
        }

        // GET: api/movimentacoes/por-moto/1
        [HttpGet("por-moto/{motoId}")]
        public async Task<ActionResult<IEnumerable<MovimentacaoDto>>> ListarPorMoto(int motoId)
        {
            var movimentacoes = await _context
                .Movimentacoes.AsNoTracking()
                .Where(m => m.MotoId == motoId)
                .OrderByDescending(m => m.DataHora)
                .Select(m => new MovimentacaoDto
                {
                    Id = m.Id,
                    MotoPlaca = m.Moto.Placa,
                    SetorAntigo = m.SetorAntigo.Nome ?? "N/A",
                    SetorNovo = m.SetorNovo.Nome ?? "N/A",
                    DataHora = m.DataHora,
                })
                .ToListAsync();

            if (!movimentacoes.Any())
                return NotFound($"Nenhuma movimentação encontrada para a moto {motoId}.");

            return Ok(movimentacoes);
        }

        // GET: api/movimentacoes/ultima/1
        [HttpGet("ultima/{motoId}")]
        public async Task<ActionResult<MovimentacaoDto>> UltimaMovimentacao(int motoId)
        {
            var ultima = await _context
                .Movimentacoes.AsNoTracking()
                .Where(m => m.MotoId == motoId)
                .OrderByDescending(m => m.DataHora)
                .Select(m => new MovimentacaoDto
                {
                    Id = m.Id,
                    MotoPlaca = m.Moto.Placa,
                    SetorAntigo = m.SetorAntigo.Nome ?? "N/A",
                    SetorNovo = m.SetorNovo.Nome ?? "N/A",
                    DataHora = m.DataHora,
                })
                .FirstOrDefaultAsync();

            if (ultima == null)
                return NotFound($"Nenhuma movimentação registrada para a moto {motoId}.");

            return Ok(ultima);
        }
    }
}
