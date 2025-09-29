using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Data;
using MotoMonitoramento.Dtos;
using MotoMonitoramento.Models;
using QRCoder;

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

            // Gerar QR Code
            string qrContent = $"https://seusite.com/api/motos/{moto.Id}"; // ou só moto.Id
            string qrCodeBase64 = GerarQRCodeBase64(qrContent);

            var response = new MotoResponseDto
            {
                Id = moto.Id,
                Placa = moto.Placa,
                SetorId = setor.Id,
                SetorNome = setor.Nome,
                QrCodeBase64 = qrCodeBase64, // novo campo no DTO
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

            // Só registra movimentação se o setor mudou
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
        public async Task<ActionResult<IEnumerable<MotoResponseDto>>> GetAll()
        {
            var motos = await _context.Motos.AsNoTracking().Include(m => m.Setor).ToListAsync(); // busca tudo primeiro

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

        // GET: api/motos/por-setor?setorId=1
        [HttpGet("por-setor")]
        public async Task<ActionResult<IEnumerable<MotoResponseDto>>> GetPorSetor(
            [FromQuery] int setorId
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

        // GET: api/motos/5
        // GET: api/motos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MotoResponseDto>> GetById(int id)
        {
            var moto = await _context
                .Motos.AsNoTracking()
                .Include(m => m.Setor)
                .FirstOrDefaultAsync(m => m.Id == id); // busca primeiro

            if (moto == null)
                return NotFound();

            var response = new MotoResponseDto
            {
                Id = moto.Id,
                Placa = moto.Placa,
                SetorId = moto.SetorId,
                SetorNome = moto.Setor?.Nome,
                QrCodeBase64 = GerarQRCodeBase64(moto.Id.ToString()), // gera QR em memória
            };

            return Ok(response);
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
