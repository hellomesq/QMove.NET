using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Data;
using MotoMonitoramento.DTOs;
using MotoMonitoramento.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MotoMonitoramento.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Gerencia operações relacionadas a setores")]
    public class SetoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SetoresController(AppDbContext context) => _context = context;

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todos os setores",
            Description = "Retorna todos os setores cadastrados no sistema"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Lista de setores retornada com sucesso",
            typeof(IEnumerable<SetorResponseDto>)
        )]
        public async Task<ActionResult<IEnumerable<SetorResponseDto>>> GetAll()
        {
            var setores = await _context
                .Setores.AsNoTracking()
                .Select(s => new SetorResponseDto { Id = s.Id, Nome = s.Nome })
                .ToListAsync();

            return Ok(setores);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra um novo setor",
            Description = "Adiciona um setor ao sistema"
        )]
        [SwaggerResponse(
            StatusCodes.Status201Created,
            "Setor cadastrado com sucesso",
            typeof(Setor)
        )]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Nome do setor inválido")]
        public async Task<ActionResult<Setor>> CadastrarSetor(
            [FromBody, SwaggerParameter("Dados do setor", Required = true)] SetorDto dto
        )
        {
            if (string.IsNullOrEmpty(dto.Nome))
                return BadRequest("Nome do setor é obrigatório.");

            var setor = new Setor { Nome = dto.Nome };
            _context.Setores.Add(setor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = setor.Id }, setor);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza um setor",
            Description = "Atualiza o nome de um setor existente"
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Setor atualizado com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Nome do setor inválido")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Setor não encontrado")]
        public async Task<IActionResult> Update(
            [FromRoute, SwaggerParameter("ID do setor", Required = true)] int id,
            [FromBody, SwaggerParameter("Dados atualizados do setor", Required = true)] SetorDto dto
        )
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
        [SwaggerOperation(Summary = "Deleta um setor", Description = "Remove um setor do sistema")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Setor removido com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Setor não encontrado")]
        public async Task<IActionResult> Delete(
            [FromRoute, SwaggerParameter("ID do setor", Required = true)] int id
        )
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
