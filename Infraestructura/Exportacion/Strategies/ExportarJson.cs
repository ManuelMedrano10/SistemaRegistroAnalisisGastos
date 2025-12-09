using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Aplicacion.Interfaces;
using Aplicacion.Servicios;
using Dominio.Excepciones;

namespace Infraestructura.Exportacion.Strategies
{
    public class ExportarJson : IExportarReporte
    {
        private readonly ReporteMensual _reporte;
        public string ContentType => "application/json";
        public string FileExtension => ".json";
        public ExportarJson(ReporteMensual reporte)
        {
            _reporte = reporte;
        }

        public byte[] ExportarReporte(int año, int mes, int idUsuario)
        {
            var resumen = _reporte.GenerarResumen(año, mes, idUsuario);

            if (idUsuario == 0)
                throw new ItemNotFoundException("El usuario no ha sido encontrado.");

            var datosOrdenados = new
            {
                resumen.Año,
                resumen.Mes,
                resumen.TotalGastado,
                TotalPorCategoria = resumen.TotalPorCategoria.OrderByDescending(kvp => kvp.Value),
                PorcentajesPorCategoria = resumen.PorcentajesPorCategoria.OrderByDescending(kvp => kvp.Value),
                ComparacionMes = resumen.ComparacionMes.OrderByDescending(kvp => kvp.Value)
            };

            var archivo = JsonSerializer.Serialize(datosOrdenados, new JsonSerializerOptions { WriteIndented = true });
            return Encoding.UTF8.GetBytes(archivo);
        }
    }
}
