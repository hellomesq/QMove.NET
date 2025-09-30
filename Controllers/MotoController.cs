using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Data;
using MotoMonitoramento.Dtos;
using MotoMonitoramento.Models;
using QRCoder;
using Swashbuckle.AspNetCore.Annotations;

namespace MotoMonitoramento.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Gerencia motos, incluindo cadastro, atualização, listagem e exclusão")]
    public class MotosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MotosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra uma nova moto",
            Description = "Recebe dados da moto e retorna a moto cadastrada com QR Code"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Moto cadastrada com sucesso",
            typeof(MotoResponseDto)
        )]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        public async Task<ActionResult<MotoResponseDto>> CadastrarMoto(
            [FromBody, SwaggerParameter("Dados da moto", Required = true)] MotoDto dto
        )
        {
            if (dto == null || string.IsNullOrEmpty(dto.Placa))
                return BadRequest("Placa é obrigatória.");

            var setor = await _context.Setores.FindAsync(dto.SetorId);
            if (setor == null)
                return BadRequest("Setor não encontrado.");

            var moto = new Moto { Placa = dto.Placa, SetorId = setor.Id };
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();

            string qrContent = moto.Id.ToString();
            string qrCodeBase64 = GerarQRCodeBase64(qrContent);

            var response = new MotoResponseDto
            {
                Id = moto.Id,
                Placa = moto.Placa,
                SetorId = setor.Id,
                SetorNome = setor.Nome,
                QrCodeBase64 = qrCodeBase64,
            };

            return Ok(response);
        }

        private string GerarQRCodeBase64(string conteudo)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(conteudo, QRCodeGenerator.ECCLevel.Q);

            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);

            return Convert.ToBase64String(qrCodeBytes);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza uma moto",
            Description = "Atualiza placa e setor de uma moto"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Moto atualizada com sucesso",
            typeof(MotoResponseDto)
        )]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Moto não encontrada")]
        public async Task<ActionResult<MotoResponseDto>> Update(
            int id,
            [FromBody, SwaggerParameter("Dados atualizados da moto", Required = true)] MotoDto dto
        )
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

            if (moto.SetorId != setor.Id)
            {
                var movimentacao = new Movimentacao
                {
                    MotoId = moto.Id,
                    SetorAntigoId = moto.SetorId ?? 0,
                    SetorNovoId = setor.Id,
                    DataHora = DateTime.UtcNow,
                };
                _context.Movimentacoes.Add(movimentacao);
            }

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

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todas as motos",
            Description = "Retorna todas as motos cadastradas"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Lista de motos retornada com sucesso",
            typeof(IEnumerable<MotoResponseDto>)
        )]
        public async Task<ActionResult<IEnumerable<MotoResponseDto>>> GetAll()
        {
            var motos = await _context.Motos.AsNoTracking().Include(m => m.Setor).ToListAsync();

            var resultado = motos
                .Select(m => new MotoResponseDto
                {
                    Id = m.Id,
                    Placa = m.Placa,
                    SetorId = m.SetorId,
                    SetorNome = m.Setor != null ? m.Setor.Nome : null,
                    QrCodeBase64 = GerarQRCodeBase64(m.Id.ToString()),
                })
                .ToList();

            return Ok(resultado);
        }

        [HttpGet("por-setor")]
        [SwaggerOperation(
            Summary = "Lista motos por setor",
            Description = "Retorna todas as motos cadastradas em um setor específico"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Lista de motos retornada com sucesso",
            typeof(IEnumerable<MotoResponseDto>)
        )]
        public async Task<ActionResult<IEnumerable<MotoResponseDto>>> GetPorSetor(
            [FromQuery, SwaggerParameter("ID do setor", Required = true)] int setorId
        )
        {
            var motos = await _context
                .Motos.AsNoTracking()
                .Where(m => m.SetorId == setorId)
                .Include(m => m.Setor)
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

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Consulta uma moto pelo ID",
            Description = "Retorna uma moto específica pelo seu ID"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Moto encontrada com sucesso",
            typeof(MotoResponseDto)
        )]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Moto não encontrada")]
        public async Task<ActionResult<MotoResponseDto>> GetById(
            [FromRoute, SwaggerParameter("ID da moto", Required = true)] int id
        )
        {
            var moto = await _context
                .Motos.AsNoTracking()
                .Include(m => m.Setor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (moto == null)
                return NotFound();

            var response = new MotoResponseDto
            {
                Id = moto.Id,
                Placa = moto.Placa,
                SetorId = moto.SetorId,
                SetorNome = moto.Setor?.Nome,
                QrCodeBase64 = GerarQRCodeBase64(moto.Id.ToString()),
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta uma moto", Description = "Remove uma moto pelo seu ID")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Moto deletada com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Moto não encontrada")]
        public async Task<IActionResult> Delete(
            [FromRoute, SwaggerParameter("ID da moto", Required = true)] int id
        )
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
