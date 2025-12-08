using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Excepciones
{
    public class NullFieldException : Exception
    {
        public NullFieldException() { }
        public NullFieldException(string message) : base(message) { }
    }
}
