using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Interfaces;
using Aplicacion.Servicios;
using CsvHelper;
using Dominio.Entidades;
using Dominio.Excepciones;

namespace Infraestructura.Exportacion.Strategies
{
    public class ExportarCsv : IExportarReporte
    {
        public string ContentType => "text/csv";
        public string FileExtension => ".csv";
        private readonly ReporteMensual _reporte;
        public ExportarCsv(ReporteMensual reporte)
        {
            _reporte = reporte;
        }

        public byte[] ExportarReporte(int año, int mes, int idUsuario)
        {
            var resumen = _reporte.GenerarResumen(año, mes, idUsuario);

            if (resumen == null)
            {
                throw new NullFieldException("Su reporte no contiene datos.");
            }

            if (idUsuario == 0)
                throw new ItemNotFoundException("El usuario no ha sido encontrado.");

            var datosOrdenados = resumen.TotalPorCategoria.OrderByDescending(kvp => kvp.Value)
                .Select(kvp => new ResumenCategoriaCsv
                {
                    Fecha = new DateTime(resumen.Año,resumen.Mes, 1),
                    Categoria = kvp.Key,
                    Gasto = kvp.Value,
                    Porcentaje = resumen.PorcentajesPorCategoria.GetValueOrDefault(kvp.Key, 0)
                }).ToList();

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using(var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(datosOrdenados);

                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}
