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

        [HttpGet("filtro/categoria")]
        public IActionResult FiltroCategoria(string categoria)
        {
            var idUsuario = ObtenerIdUsuario();

            var gastos = _gastoServices.GetGastoPorCategoria(categoria, idUsuario);
            return Ok(gastos);
        }

        [HttpGet("filtro/metodopago")]
        public IActionResult FiltroMetodoPago(string metodoPago)
        {
            var idUsuario = ObtenerIdUsuario();

            var gastos = _gastoServices.GetGastoPorMetodoPago(metodoPago, idUsuario);
            return Ok(gastos);
        }

        [HttpGet("filtro/descripcion")]
        public IActionResult FiltroDescripcion(string descripcion)
        {
            var idUsuario = ObtenerIdUsuario();

            var gastos = _gastoServices.GetGastoPorDescripcion(descripcion, idUsuario);
            return Ok(gastos);
        }

        [HttpGet("filtro/fechas")]
        public IActionResult FiltroFechas(DateTime inicio, DateTime fin)
        {
            var idUsuario = ObtenerIdUsuario();

            var gastos = _gastoServices.GetGastoPorFechas(inicio, fin, idUsuario);
            return Ok(gastos);
        }

        [HttpGet("{id}")]
        public IActionResult Obtener(int id)
        {
            var idUsuario = ObtenerIdUsuario();
            var gasto = _gastoServices.ObtenerPorId(id, idUsuario);
            return Ok(gasto);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] GastoCreateDto dto)
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
        
        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] GastoUpdateDto dto)
        {
            var idUsuario = ObtenerIdUsuario();
            if (id != dto.Id)
                return BadRequest(new { message = "Los IDs de los gastos no coinciden." });

            _gastoServices.Update(dto, idUsuario);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            var idUsuario = ObtenerIdUsuario();
            _gastoServices.Delete(id, idUsuario);
            return NoContent();
        }

        [HttpPost("importar")]
        public IActionResult ImportarGastos(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest(new { message = "No se ha enviado ningun archivo." });

            var extension = Path.GetExtension(archivo.FileName).ToLower();
            if (extension != ".xlsx")
                return BadRequest(new { message = $"El formato. {extension} no es valido. Por favor, importe un archivo .xlsx." });

            var mimeTypesValidos = new[] { 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "application/vnd.ms-excel" 
            };

            if (!mimeTypesValidos.Contains(archivo.ContentType))
                return BadRequest(new { message = "Su archivo Excel no es valido." });

            var idUsuario = ObtenerIdUsuario();

            try
            {
                using(var stream = archivo.OpenReadStream())
                {
                    _gastoServices.ImportarGastoExcel(stream, idUsuario);
                }
                
                return Ok("El archivo ha sido importado correctamente. Sus gastos han sido guardados.");
            }catch(Exception ex)
            {
                return BadRequest(new { message = $"Error al procesar el archivo: {ex.Message}" });
            }
        }
    }
}
