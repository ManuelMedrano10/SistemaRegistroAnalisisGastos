using Aplicacion.DTOs.Gasto;
using Aplicacion.Servicios;
using Dominio.Excepciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.OpenXmlFormats.Dml;

namespace Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GastoController : ControllerBase
    {
        private readonly GastoServices _gastoServices;
        private readonly CategoriaServices _categoriaServices;
        private readonly MetodoPagoServices _metodoPagoServices;
        public GastoController(GastoServices gastoServices, CategoriaServices categoriaServices, MetodoPagoServices metodoPagoServices)
        {
            _gastoServices = gastoServices;
            _categoriaServices = categoriaServices;
            _metodoPagoServices = metodoPagoServices;
        }
        private int ObtenerIdUsuario()
        {
            int idUsuario = int.Parse(User.FindFirst("Id").Value);

            return idUsuario;
        }

        [HttpGet]
        public IActionResult ObtenerGastos()
        {
            var idUsuario = ObtenerIdUsuario();
            var gastos = _gastoServices.GetAll(idUsuario);
            return Ok(gastos);
        }

        [HttpGet]
        public IActionResult FiltroCategoria(string categoria)
        {
            var idUsuario = ObtenerIdUsuario();

            var gastos = _gastoServices.GetGastoPorCategoria(categoria, idUsuario);
            return Ok(gastos);
        }

        [HttpGet]
        public IActionResult FiltroMetodoPago(string metodoPago)
        {
            var idUsuario = ObtenerIdUsuario();

            var gastos = _gastoServices.GetGastoPorMetodoPago(metodoPago, idUsuario);
            return Ok(gastos);
        }

        [HttpGet]
        public IActionResult FiltroDescripcion(string descripcion)
        {
            var idUsuario = ObtenerIdUsuario();

            var gastos = _gastoServices.GetGastoPorDescripcion(descripcion, idUsuario);
            return Ok(gastos);
        }

        [HttpGet]
        public IActionResult FiltroFechas(DateTime inicio, DateTime fin)
        {
            var idUsuario = ObtenerIdUsuario();

            var gastos = _gastoServices.GetGastoPorFechas(inicio, fin, idUsuario);
            return Ok(gastos);
        }

        [HttpGet]
        public IActionResult Obtener(int id)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();
                var gasto = _gastoServices.ObtenerPorId(id, idUsuario);

                return Ok(gasto);
            }
            catch (ItemNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Crear([FromBody] GastoCreateDto dto)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();
                var gasto = _gastoServices.Create(dto, idUsuario);

                var gastoResponse = new GastoDto
                {
                    Id = gasto.Id,
                    Monto = gasto.Monto,
                    Fecha = gasto.Fecha,
                    Descripcion = gasto.Descripcion,
                    Categoria = _categoriaServices.ObtenerPorId(gasto.IdCategoria, idUsuario).Nombre,
                    MetodoPago = _metodoPagoServices.ObtenerPorId(gasto.IdMetodoPago, idUsuario).Nombre
                };

                return CreatedAtAction(nameof(Obtener), new { Id = gasto.Id}, gastoResponse);
            }
            catch(MontoInvalidoException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ItemNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] GastoUpdateDto dto)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();
                if (id != dto.Id)
                    return BadRequest(new { message = "Los IDs de los gastos no coinciden." });

                _gastoServices.Update(dto, idUsuario);
                return NoContent();
            }
            catch (MontoInvalidoException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ItemNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();
                _gastoServices.Delete(id, idUsuario);
                return NoContent();
            }
            catch (ItemNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("importar")]
        public IActionResult ImportarGastos(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest(new { message = "No se ha enviado ningun archivo." });

            var extension = Path.GetExtension(archivo.FileName).ToLower();
            if (extension != ".csv")
                return BadRequest(new { message = $"El formato. {extension} no es valido. Por favor, importe un archivo .csv." });

            var mimeTypesValidos = new[] { "text/csv", "application/csv", "application/vnd.ms-excel", "text/plain" };
            if (!mimeTypesValidos.Contains(archivo.ContentType))
                return BadRequest(new { message = "Su archivo CSV no es valido." });

            try
            {
                var idUsuario = ObtenerIdUsuario();
                _gastoServices.ImportarGastoCsv(archivo.OpenReadStream(), idUsuario);
                return Ok("El archivo ha sido importado correctamente. Sus gastos han sido guardados.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
