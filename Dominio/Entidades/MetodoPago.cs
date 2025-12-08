using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class MetodoPago
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int IdUsuario { get; set; }
    }
}
