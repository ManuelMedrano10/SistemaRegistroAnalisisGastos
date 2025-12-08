using System.ComponentModel.DataAnnotations;

namespace Dominio.Entidades
{
    public class Gasto
    {
        [Key]
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public int IdCategoria { get; set; }
        public int IdMetodoPago { get; set; }
        public int IdUsuario { get; set; }
    }
}
