using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Usuario
{
    public class UsuarioRegisterDto
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Clave { get; set; }
    }
}
