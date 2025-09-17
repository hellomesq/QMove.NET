using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Data;
using MotoMonitoramento.Models;

namespace MotoMonitoramento.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SetoresController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Setor>>> GetAll() =>
            await _context.Setores.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Setor>> Create(Setor setor)
        {
            _context.Setores.Add(setor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = setor.Id }, setor);
        }
    }
}
