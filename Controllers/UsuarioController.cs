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
    [SwaggerTag("Gerencia operações relacionadas a usuários")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todos os usuários",
            Description = "Retorna todos os usuários cadastrados"
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Lista de usuários retornada com sucesso",
            typeof(IEnumerable<Usuario>)
        )]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Busca usuário por ID",
            Description = "Retorna um usuário específico pelo seu ID"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Usuário encontrado", typeof(Usuario))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Usuário não encontrado")]
        public async Task<ActionResult<Usuario>> GetUsuario(
            [FromRoute, SwaggerParameter("ID do usuário", Required = true)] int id
        )
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();
            return usuario;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra um novo usuário",
            Description = "Adiciona um novo usuário ao sistema"
        )]
        [SwaggerResponse(
            StatusCodes.Status201Created,
            "Usuário criado com sucesso",
            typeof(Usuario)
        )]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        public async Task<ActionResult<Usuario>> PostUsuario(
            [FromBody, SwaggerParameter("Dados do usuário", Required = true)] UsuarioDto dto
        )
        {
            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza um usuário",
            Description = "Altera os dados de um usuário existente"
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Usuário atualizado com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Usuário não encontrado")]
        public async Task<IActionResult> PutUsuario(
            [FromRoute, SwaggerParameter("ID do usuário", Required = true)] int id,
            [FromBody, SwaggerParameter("Dados atualizados do usuário", Required = true)]
                UsuarioDto dto
        )
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.Senha = dto.Senha;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deleta um usuário",
            Description = "Remove um usuário do sistema"
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Usuário removido com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Usuário não encontrado")]
        public async Task<IActionResult> DeleteUsuario(
            [FromRoute, SwaggerParameter("ID do usuário", Required = true)] int id
        )
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
