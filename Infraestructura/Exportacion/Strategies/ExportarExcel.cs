using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Interfaces;
using Aplicacion.Servicios;
using ClosedXML.Excel;
using CsvHelper;
using Dominio.Entidades;
using Dominio.Excepciones;

namespace Infraestructura.Exportacion.Strategies
{
    public class ExportarExcel : IExportarReporte
    {
        public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public string FileExtension => ".xlsx";
        private readonly ReporteMensual _reporte;
        public ExportarExcel(ReporteMensual reporte)
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

            using(var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte Mensual");

                worksheet.Cell(1, 1).Value = "Fecha";
                worksheet.Cell(1, 2).Value = "Categoría";
                worksheet.Cell(1, 3).Value = "Gasto Total";
                worksheet.Cell(1, 4).Value = "Porcentaje";

                var rangoHeader = worksheet.Range("A1:D1");
                rangoHeader.Style.Font.Bold = true;
                rangoHeader.Style.Fill.BackgroundColor = XLColor.LightGray;

                for (int i = 0; i < datosOrdenados.Count; i++)
                {
                    var fila = i + 2;
                    var dato = datosOrdenados[i];

                    worksheet.Cell(fila, 1).Value = dato.Fecha;
                    worksheet.Cell(fila, 2).Value = dato.Categoria;
                    worksheet.Cell(fila, 3).Value = dato.Gasto;
                    worksheet.Cell(fila, 4).Value = dato.Porcentaje / 100.0m;
                }

                worksheet.Column(3).Style.NumberFormat.Format = "$ #,##0.00";
                worksheet.Column(4).Style.NumberFormat.Format = "0.00%";
                worksheet.Columns().AdjustToContents();

                using(var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
