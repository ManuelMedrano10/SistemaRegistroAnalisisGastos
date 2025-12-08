using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs
{
    public class GastoCreateDto
    {
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public int IdCategoria { get; set; }
        public int IdMetodoPago { get; set; }
    }
}
