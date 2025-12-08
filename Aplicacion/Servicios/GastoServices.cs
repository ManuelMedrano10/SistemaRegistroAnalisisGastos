using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.DTOs;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Excepciones;

namespace Aplicacion.Servicios
{
    public class GastoServices
    {
        private readonly IRepository<Gasto> _repoGasto;
        private readonly IRepository<Categoria> _repoCategoria;
        private readonly IRepository<MetodoPago> _repoMetodoPago;
        public GastoServices(IRepository<Gasto> repoG, IRepository<Categoria> repoC, IRepository<MetodoPago> repoM)
        {
            _repoGasto = repoG;
            _repoCategoria = repoC;
            _repoMetodoPago = repoM;
        }

        public IEnumerable<GastoDto> GetAll(int idUsuario)
        {
            return _repoGasto.GetAll().Select(g => new GastoDto
            {
                Id = g.Id,
                Monto = g.Monto,
                Fecha = g.Fecha,
                Descripcion = g.Descripcion,
                Categoria = _repoCategoria.Get(g.IdCategoria).Nombre,
                MetodoPago = _repoMetodoPago.Get(g.IdMetodoPago).Nombre
            }).ToList().Where(g => _repoGasto.Get(g.Id).IdUsuario == idUsuario);
        }

        public IEnumerable<Gasto> ObtenerGastoPorcategoriaID(int idCategoria)
        {
            var gastos = _repoGasto.GetAll();
            return gastos.Where(g => g.IdCategoria == idCategoria).ToList();
        }

        public IEnumerable<GastoDto> GetGastoPorCategoria(string categoria, int idUsuario)
        {
            var gastos = GetAll(idUsuario);
            return gastos.Where(g => g.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<GastoDto> GetGastoPorMetodoPago(string metodoPago, int idUsuario)
        {
            var gastos = GetAll(idUsuario);
            return gastos.Where(g => g.MetodoPago.Equals(metodoPago, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<GastoDto> GetGastoPorDescripcion(string descripcion, int idUsuario)
        {
            var gastos = GetAll(idUsuario);
            return gastos.Where(g => g.MetodoPago.Contains(descripcion, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<GastoDto> GetGastoPorFechas(DateTime inicio, DateTime fin, int idUsuario)
        {
            var gastos = GetAll(idUsuario);
            return gastos.Where(g => g.Fecha >= inicio && g.Fecha <= fin).ToList();
        }   

        public IEnumerable<GastoDto> ObtenerGastosMes(int año, int mes, int idUsuario)
        {
            var inicio = new DateTime(año, mes, 1);
            var fin = inicio.AddMonths(1).AddDays(-1);
            return GetGastoPorFechas(inicio, fin, idUsuario);
        }

        public GastoDto ObtenerPorId(int id, int idUsuario)
        {
            var gasto = _repoGasto.Get(id);

            if (gasto.IdUsuario == idUsuario)
            {
                return new GastoDto
                {
                    Id = gasto.Id,
                    Monto = gasto.Monto,
                    Fecha = gasto.Fecha,
                    Descripcion = gasto.Descripcion,
                    Categoria = _repoCategoria.Get(gasto.IdCategoria).Nombre,
                    MetodoPago = _repoMetodoPago.Get(gasto.IdMetodoPago).Nombre
                };
            }
            else
            {
                throw new ItemNotFoundException("Su gasto no ha sido encontrado.");
            }
        }

        public void Create(GastoCreateDto dto, int idUsuario)
        {
            if (dto.Monto < 0)
                throw new MontoInvalidoException("Su monto es invalido. Debe ser positivo o diferente de 0.");
            if (string.IsNullOrEmpty(dto.Descripcion))
                dto.Descripcion = "";
            if (_repoCategoria.Get(dto.IdCategoria) == null)
                throw new ItemNotFoundException("La categoria insertada no se ha encontrado.");
            if (_repoMetodoPago.Get(dto.IdMetodoPago) == null)
                throw new ItemNotFoundException("El metodo de pago insertadono se ha encontrado.");

            var gasto = new Gasto
            {
                Monto = dto.Monto,
                Fecha = dto.Fecha,
                Descripcion = dto.Descripcion,
                IdCategoria = dto.IdCategoria,
                IdMetodoPago = dto.IdMetodoPago,
                IdUsuario = idUsuario
            };

            var categoriaAsociada = _repoCategoria.Get(gasto.IdCategoria);
            
            if(categoriaAsociada.IsActivo == false)
            {
                categoriaAsociada.IsActivo = true;
                _repoCategoria.Update(categoriaAsociada);
            }
            _repoGasto.Add(gasto);
        }

        public void Update(GastoUpdateDto dto, int idUsuario)
        {
            if (dto.Monto < 0)
                throw new MontoInvalidoException("Su monto es invalido. Debe ser positivo o diferente de 0.");
            if (string.IsNullOrEmpty(dto.Descripcion))
                dto.Descripcion = "";
            if (_repoCategoria.Get(dto.IdCategoria) == null)
                throw new ItemNotFoundException("La categoria insertada no se ha encontrado.");
            if (_repoMetodoPago.Get(dto.IdMetodoPago) == null)
                throw new ItemNotFoundException("El metodo de pago insertadono se ha encontrado.");

            var gasto = new Gasto
            {
                Monto = dto.Monto,
                Fecha = dto.Fecha,
                Descripcion = dto.Descripcion,
                IdCategoria = dto.IdCategoria,
                IdMetodoPago = dto.IdMetodoPago,
                IdUsuario = idUsuario
            };

            _repoGasto.Update(gasto);
        }

        public void Delete(int id, int idUsuario)
        {
            var gastoExistente = _repoGasto.Get(id);
            if (gastoExistente.Id != idUsuario)
                throw new ItemNotFoundException("Su gasto no ha sido encontrado.");
            _repoGasto.Delete(id);
        }
    }
}
