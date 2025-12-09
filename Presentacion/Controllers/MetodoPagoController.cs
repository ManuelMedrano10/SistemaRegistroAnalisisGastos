using Aplicacion.DTOs.MetodoPago;
using Aplicacion.Servicios;
using Dominio.Excepciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MetodoPagoController : ControllerBase
    {
        private readonly MetodoPagoServices _metodoServices;
        public MetodoPagoController(MetodoPagoServices metodoServices)
        {
            _metodoServices = metodoServices;
        }

        private int ObtenerIdUsuario()
        {
            int idUsuario = int.Parse(User.FindFirst("Id").Value);

            return idUsuario;
        }

        [HttpGet]
        public IActionResult GetMetodosPago()
        {
            var idUsuario = ObtenerIdUsuario();
            var metodoPagos = _metodoServices.ObtenerMetodosPago(idUsuario);
            return Ok(metodoPagos);
        }

        [HttpGet("{id}")]
        public IActionResult Obtener(int id)
        {
            var idUsuario = ObtenerIdUsuario();
            var metodoPago = _metodoServices.ObtenerPorId(id, idUsuario);

            if (metodoPago == null)
                return NotFound();

            return Ok(metodoPago);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] MetodoPagoCreateDto dto)
        {
           var idUsuario = ObtenerIdUsuario();
           _metodoServices.Create(dto, idUsuario);
           return CreatedAtAction(nameof(Obtener), new { id = _metodoServices.ObtenerPorNombre(dto.Nombre).Id,}, dto);
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] MetodoPagoDto dto)
        {
            var idUsuario = ObtenerIdUsuario();
            if (id != dto.Id)
                return BadRequest(new { message = "Los IDs de los metodos de pago no coinciden." });
            _metodoServices.Update(dto, idUsuario);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            var idUsuario = ObtenerIdUsuario();
            _metodoServices.Delete(id, idUsuario);
            return NoContent();
        }
    }
}
