using Microsoft.AspNetCore.Mvc;

namespace MotoMonitoramento.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class TesteController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok("API versão 1 funcionando corretamente!");
    }
}
