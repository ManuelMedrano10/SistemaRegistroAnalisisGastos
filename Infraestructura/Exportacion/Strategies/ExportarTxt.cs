using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Interfaces;
using Aplicacion.Servicios;
using Dominio.Entidades;

namespace Infraestructura.Exportacion.Strategies
{
    public class ExportarTxt : IExportarReporte
    {
        private readonly ReporteMensual _reporte;
        public string ContentType => "text/plain";
        public string FileExtension => ".txt";
        public ExportarTxt(ReporteMensual reporte)
        {
            _reporte = reporte;
        }

        public byte[] ExportarReporte(int año, int mes, int idUsuario)
        {
            var resumen = _reporte.GenerarResumen(año, mes, idUsuario);
            var sb = new StringBuilder();

            sb.AppendLine($"Año: {resumen.Año}");
            sb.AppendLine($"\nMes: {resumen.Mes}");
            sb.AppendLine($"\nTotal Gastado: {resumen.TotalGastado:C}");
            
            sb.AppendLine($"\nTotal Por Categoria:");
            var categoriasOrdenadas = resumen.TotalPorCategoria.OrderByDescending(r => r.Value);
            foreach(var item in categoriasOrdenadas)
            {
                sb.AppendLine($"{item.Key}: {item.Value:C}");
            }

            sb.AppendLine($"Porcentaje Por Categoria:");
            var porcentajesOrdenados = resumen.PorcentajesPorCategoria.OrderByDescending(p => p.Value);
            foreach (var item in porcentajesOrdenados)
            {
                sb.AppendLine($"{item.Key}: {item.Value:C}");
            }

            sb.AppendLine($"Comparacion Con Mes Anterior:");
            var comparacionesOrdenadas = resumen.ComparacionMes.OrderByDescending(o => o.Value);
            foreach (var item in comparacionesOrdenadas)
            {
                sb.AppendLine($"{item.Key}: {item.Value:C}");
            };

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
