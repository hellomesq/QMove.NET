using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MotoMonitoramento.Data;
using MotoMonitoramento.DTOs;
using MotoMonitoramento.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MotoMonitoramento.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [SwaggerTag("Gerencia operações relacionadas a usuários")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // =================== LOGIN ===================
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.Email == dto.Email && u.Senha == dto.Senha
            );

            if (usuario == null)
                return Unauthorized("Email ou senha inválidos.");

            // Pega a chave do JWT via IConfiguration
            var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var keyString = config["Jwt:Key"];
            var issuer = config["Jwt:Issuer"];

            if (string.IsNullOrWhiteSpace(keyString) || string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("JWT Key ou Issuer não definidos!");

            var key = Encoding.UTF8.GetBytes(keyString);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim(ClaimTypes.Name, usuario.Nome),
                        new Claim(ClaimTypes.Email, usuario.Email),
                    }
                ),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = issuer,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                ),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { Token = jwt });
        }

        // =================== ENDPOINTS ===================
        [HttpGet]
        [Authorize] // agora precisa de token para acessar
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
        [Authorize]
        [SwaggerOperation(
            Summary = "Busca usuário por ID",
            Description = "Retorna um usuário específico pelo seu ID"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Usuário encontrado", typeof(Usuario))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Usuário não encontrado")]
        public async Task<ActionResult<Usuario>> GetUsuario([FromRoute] int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();
            return usuario;
        }

        [HttpPost]
        [AllowAnonymous]
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
        public async Task<ActionResult<Usuario>> PostUsuario([FromBody] UsuarioDto dto)
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
        [Authorize]
        [SwaggerOperation(
            Summary = "Atualiza um usuário",
            Description = "Altera os dados de um usuário existente"
        )]
        public async Task<IActionResult> PutUsuario(int id, [FromBody] UsuarioDto dto)
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
        [Authorize]
        [SwaggerOperation(
            Summary = "Deleta um usuário",
            Description = "Remove um usuário do sistema"
        )]
        public async Task<IActionResult> DeleteUsuario(int id)
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
