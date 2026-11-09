using Microsoft.AspNetCore.Mvc;
using WebApi_Solangel_Medina.Models;
using WebApi_Solangel_Medina.Services;

namespace WebApi_Solangel_Medina.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotizacionController : ControllerBase
    {
        private readonly CotizacionService _service;

        public CotizacionController(CotizacionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            return Ok(await _service.ObtenerTodos());
        }

        [HttpGet("solicitud/{solicitudId}")]
        public async Task<IActionResult> ObtenerPorSolicitud(int solicitudId)
        {
            return Ok(
                await _service.ObtenerPorSolicitud(solicitudId)
            );
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Cotizacion cotizacion)
        {
            int id = await _service.Crear(cotizacion);

            return Ok(new
            {
                Id = id,
                Mensaje = "Cotización registrada correctamente"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            bool resultado = await _service.Eliminar(id);

            if (!resultado)
                return NotFound("Cotización no encontrada");

            return Ok("Cotización eliminada correctamente");
        }
    }
}
