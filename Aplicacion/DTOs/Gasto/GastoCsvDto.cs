using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Gasto
{
    public class GastoCsvDto
    {
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public string MetodoPago { get; set; }
    }
}
