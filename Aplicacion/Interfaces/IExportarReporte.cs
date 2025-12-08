using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;

namespace Aplicacion.Interfaces
{
    public interface IExportarReporte
    {
        string ContentType { get; }
        string FileExtension { get; }
        byte[] ExportarReporte(int año, int mes, int idUsuario);
    }
}
