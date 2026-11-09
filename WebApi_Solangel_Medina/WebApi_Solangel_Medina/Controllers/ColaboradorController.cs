using Microsoft.AspNetCore.Mvc;
using WebApi_Solangel_Medina.Services;

namespace WebApi_Solangel_Medina.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColaboradorController : ControllerBase
    {
        private readonly ColaboradorService _service;

        public ColaboradorController(ColaboradorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            return Ok(await _service.ObtenerTodos());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var colaborador = await _service.ObtenerPorId(id);

            if (colaborador == null)
                return NotFound("Colaborador no encontrado");

            return Ok(colaborador);
        }
    }
}
