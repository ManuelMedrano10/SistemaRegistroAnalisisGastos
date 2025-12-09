using Aplicacion.Interfaces;
using Aplicacion.Servicios;
using Dominio.Excepciones;
using Infraestructura.Exportacion.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExportarReporteController : ControllerBase
    {
        private readonly ReporteMensual _reporte;
        public ExportarReporteController(ReporteMensual reporte)
        {
            _reporte = reporte;
        }

        private int ObtenerIdUsuario()
        {
            int idUsuario = int.Parse(User.FindFirst("Id").Value);

            return idUsuario;
        }

        [HttpGet("descargar/{format}")]
        public IActionResult ExportarReporte(string format, int año, int mes)
        {
            int idUsuario = ObtenerIdUsuario();
            try
            {
                var exportarFactory = new ExportadorFactory(_reporte);
                IExportarReporte exportar = exportarFactory.ObtenerFormato(format);

                byte[] archivo = exportar.ExportarReporte(año, mes, idUsuario);
                string nombreArchivo = $"Reporte_Mensual_{año}_{mes}{exportar.FileExtension}";

                return File(archivo, exportar.ContentType, nombreArchivo);
            }
            catch(FormatoInvalidoException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(NullFieldException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Se ha producido un error al intentar generar el reporte.");
            }
        }
    }
}
