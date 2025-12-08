using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Excepciones
{
    public class EmailAlreadyInUseException : Exception
    {
        public EmailAlreadyInUseException() { }
        public EmailAlreadyInUseException(string message) : base(message) { }
    }
}
