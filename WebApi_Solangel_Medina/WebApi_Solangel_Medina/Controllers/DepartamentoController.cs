using Microsoft.AspNetCore.Mvc;
using WebApi_Solangel_Medina.Services;

namespace WebApi_Solangel_Medina.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartamentoController: ControllerBase
    {

        private readonly DepartamentoService _service;

        public DepartamentoController(DepartamentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var departamentos = await _service.ObtenerTodos();
            return Ok(departamentos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var departamento = await _service.ObtenerPorId(id);

            if (departamento == null)
                return NotFound("Departamento no encontrado");

            return Ok(departamento);
        }
    }
}
