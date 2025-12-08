using Aplicacion.Interfaces;
using Infraestructura.Persistencia.Contexto;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Persistencia.Repositorios
{
    public class Repositorio<T> : IRepository<T> where T : class
    {
        private readonly P2Context _context;
        private readonly DbSet<T> dbSet;

        public Repositorio(P2Context context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public void Add(T entidad)
        {
            dbSet.Add(entidad);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entidad = Get(id);
            dbSet.Remove(entidad);
            _context.SaveChanges();
        }

        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public void Update(T entidad)
        {
            dbSet.Entry(entidad).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
