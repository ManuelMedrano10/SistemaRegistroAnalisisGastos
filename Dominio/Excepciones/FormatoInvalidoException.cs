using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Excepciones
{
    public class FormatoInvalidoException : Exception
    {
        public FormatoInvalidoException() { }
        public FormatoInvalidoException(string message) : base(message) { }
    }
}
