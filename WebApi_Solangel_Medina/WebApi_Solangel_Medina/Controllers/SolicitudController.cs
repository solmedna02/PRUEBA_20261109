using Microsoft.AspNetCore.Mvc;
using WebApi_Solangel_Medina.Dtos;
using WebApi_Solangel_Medina.Services;

namespace WebApi_Solangel_Medina.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudController : ControllerBase
    {
        private readonly SolicitudService _service;

        public SolicitudController(SolicitudService service)
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
            var solicitud = await _service.ObtenerPorId(id);

            if (solicitud == null)
                return NotFound("Solicitud no encontrada");

            var detalles = await _service.ObtenerDetalles(id);

            return Ok(new
            {
                Solicitud = solicitud,
                Detalles = detalles
            });
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearSolicitud dto)
        {
            try
            {
                int id = await _service.Crear(dto);

                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id },
                    new { Id = id, Mensaje = "Solicitud creada" }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/enviar")]
        public async Task<IActionResult> EnviarRevision(int id)
        {
            bool resultado = await _service.EnviarRevision(id);

            if (!resultado)
                return BadRequest("No se puede enviar la solicitud");

            return Ok("Solicitud enviada a revisión");
        }

        [HttpPut("{id}/aprobar/{etapa}")]
        public async Task<IActionResult> Aprobar(int id, string etapa)
        {
            bool resultado = await _service.Aprobar(id, etapa);

            if (!resultado)
                return BadRequest("No se puede aprobar esta etapa");

            return Ok("Etapa aprobada correctamente");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(
    int id, CrearSolicitud dto)
        {
            try
            {
                bool resultado = await _service.Actualizar(id, dto);

                if (!resultado)
                    return BadRequest(
                        "La solicitud no existe o no está en borrador"
                    );

                return Ok("Solicitud actualizada correctamente");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            bool resultado = await _service.Eliminar(id);

            if (!resultado)
                return BadRequest(
                    "La solicitud no existe o no está en borrador"
                );

            return Ok("Solicitud eliminada correctamente");
        }

        [HttpPut("{id}/rechazar")]
        public async Task<IActionResult> Rechazar(int id)
        {
            bool resultado = await _service.Rechazar(id);

            if (!resultado)
                return BadRequest("No se puede rechazar la solicitud");

            return Ok("Solicitud rechazada correctamente");
        }
    }
}
