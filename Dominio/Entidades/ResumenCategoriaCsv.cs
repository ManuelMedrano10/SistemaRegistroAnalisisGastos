using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class ResumenCategoriaCsv
    {
        public DateTime Fecha { get; set; }
        public string Categoria { get; set; }
        public decimal Gasto { get; set; }
        public decimal Porcentaje { get; set; }
    }
}
