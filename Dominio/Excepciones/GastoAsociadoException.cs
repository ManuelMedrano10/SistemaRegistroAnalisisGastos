using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Excepciones
{
    public class GastoAsociadoException : Exception
    {
        public GastoAsociadoException(){}
        public GastoAsociadoException(string message): base(message){}
    }
}
