using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class ResumenMensual
    {   
        public int Año { get; set; }
        public int Mes { get; set; }
        public decimal TotalGastado { get; set; }
        public Dictionary<string, decimal> TotalPorCategoria { get; set; }
        public Dictionary<string, decimal> PorcentajesPorCategoria { get; set; }
        public Dictionary<string, decimal> ComparacionMes { get; set; }

        public ResumenMensual(int año, int mes, decimal totalGastado)
        {
            Año = año;
            Mes = mes;
            TotalGastado = totalGastado;
            TotalPorCategoria = new Dictionary<string, decimal>();
            PorcentajesPorCategoria = new Dictionary<string, decimal>();
            ComparacionMes = new Dictionary<string, decimal>();
        }
    }
}
