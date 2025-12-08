using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Persistencia.Contexto
{
    public class P2Context : DbContext
    {
        public P2Context(DbContextOptions<P2Context> db) : base(db) { }

        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Gasto> Gasto { get; set; }
        public DbSet<MetodoPago> MetodoPago { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
    }
}
