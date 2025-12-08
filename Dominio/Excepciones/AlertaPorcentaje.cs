using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Excepciones
{
    public class AlertaPorcentaje : Exception
    {
        public AlertaPorcentaje()
        {
            
        }
        public AlertaPorcentaje(string message)
            : base(message)
        {
            
        }
    }
}
