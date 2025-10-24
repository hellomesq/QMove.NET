using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Data;
using MotoMonitoramento.Dtos;
using MotoMonitoramento.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MotoMonitoramento.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [SwaggerTag("Gerencia movimentações de motos entre setores")]
    public class MovimentacoesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MovimentacoesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("movimentacoes")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Registra uma movimentação",
            Description = "Registra a movimentação de uma moto para um novo setor"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Movimentação registrada com sucesso",
            typeof(MovimentacaoDto)
        )]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Moto ou setor não encontrado")]
        public async Task<ActionResult<MovimentacaoDto>> RegistrarMovimentacao(
            [FromQuery, SwaggerParameter("ID da moto", Required = true)] int motoId,
            [FromQuery, SwaggerParameter("ID do novo setor", Required = true)] int novoSetorId
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

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todas as movimentações",
            Description = "Retorna todas as movimentações registradas"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Lista de movimentações retornada com sucesso",
            typeof(IEnumerable<MovimentacaoDto>)
        )]
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

        [HttpGet("por-moto/{motoId}")]
        [SwaggerOperation(
            Summary = "Lista movimentações por moto",
            Description = "Retorna todas as movimentações de uma moto específica"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Lista de movimentações retornada com sucesso",
            typeof(IEnumerable<MovimentacaoDto>)
        )]
        [SwaggerResponse(
            StatusCodes.Status404NotFound,
            "Nenhuma movimentação encontrada para a moto"
        )]
        public async Task<ActionResult<IEnumerable<MovimentacaoDto>>> ListarPorMoto(
            [FromRoute, SwaggerParameter("ID da moto", Required = true)] int motoId
        )
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

        [HttpGet("ultima/{motoId}")]
        [SwaggerOperation(
            Summary = "Última movimentação",
            Description = "Retorna a última movimentação registrada de uma moto"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Movimentação retornada com sucesso",
            typeof(MovimentacaoDto)
        )]
        [SwaggerResponse(
            StatusCodes.Status404NotFound,
            "Nenhuma movimentação encontrada para a moto"
        )]
        public async Task<ActionResult<MovimentacaoDto>> UltimaMovimentacao(
            [FromRoute, SwaggerParameter("ID da moto", Required = true)] int motoId
        )
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
