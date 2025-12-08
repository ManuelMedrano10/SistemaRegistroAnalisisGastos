using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Interfaces;
using Aplicacion.Servicios;
using Dominio.Excepciones;
using Infraestructura.Exportacion.Strategies;

namespace Infraestructura.Exportacion.Factory
{
    public class ExportadorFactory
    {
        private readonly ReporteMensual _reporte;
        public ExportadorFactory(ReporteMensual reporte)
        {
            _reporte = reporte ?? throw new ArgumentNullException(nameof(reporte));
        }
        public IExportarReporte ObtenerFormato(string format)
        {
            switch (format.ToUpper())
            {
                case "JSON":
                    return new ExportarJson(_reporte);

                case "CSV":
                    return new ExportarCsv(_reporte);
                
                case "TXT":
                    return new ExportarTxt(_reporte);

                default:
                    throw new FormatoInvalidoException("El formato NO es compatible.");
            }
        }
    }
}
