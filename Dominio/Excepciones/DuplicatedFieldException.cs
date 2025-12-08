using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Excepciones
{
    public class DuplicatedFieldException : Exception
    {
        public DuplicatedFieldException() {}
        public DuplicatedFieldException(string message) : base(message) {}
    }
}
