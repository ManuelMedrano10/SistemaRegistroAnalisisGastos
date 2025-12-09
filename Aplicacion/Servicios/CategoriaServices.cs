using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.DTOs;
using Aplicacion.DTOs.Categoria;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Excepciones;

namespace Aplicacion.Servicios
{
    public class CategoriaServices
    {
        private readonly IRepository<Categoria> _repo;
        private readonly GastoServices gastoServices;
        public CategoriaServices(IRepository<Categoria> repo, GastoServices gastoServices)
        {
            _repo = repo;
            this.gastoServices = gastoServices;
        }

        public IEnumerable<CategoriaDto> GetAll(int idUsuario)
        {
            return _repo.GetAll().Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Presupuesto = c.Presupuesto,
                IsActivo = c.IsActivo
            }).ToList().Where(c => _repo.Get(c.Id).IdUsuario == idUsuario);
        }

        public IEnumerable<CategoriaDto> GetCategoriasActivas(int idUsuario)
        {
            var categorias = GetAll(idUsuario);
            return categorias.Where(c => c.IsActivo == true);
        }

        public CategoriaDto ObtenerPorId(int id, int idUsuario)
        {
            var categoria = _repo.Get(id);
            if (categoria.IdUsuario == idUsuario)
            {
                return new CategoriaDto
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre,
                    Presupuesto = categoria.Presupuesto,
                    IsActivo = categoria.IsActivo
                };
            }
            else
            {
                throw new ItemNotFoundException("Su categoria no ha sido encontrada.");
            }
        }

        public Categoria ObtenerPorNombre(string nombre)
        {
            var categorias = _repo.GetAll();
            return categorias.FirstOrDefault(c => c.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }

        public void Create(CategoriaCreateDto dto, int idUsuario)
        {
            if (string.IsNullOrEmpty(dto.Nombre))
                throw new NullFieldException("La categoria debe tener un nombre.");
            if (dto.Presupuesto < 0)
                throw new MontoInvalidoException("El presupuesto de la categoria debe ser positivo.");
            if (ObtenerPorNombre(dto.Nombre) != null)
                throw new DuplicatedFieldException("Las categorias no deben estar duplicadas.");

            var categoria = new Categoria
            {
                Nombre = dto.Nombre,
                Presupuesto = dto.Presupuesto,
                IsActivo = false,
                IdUsuario = idUsuario
            };

            _repo.Add(categoria);
        }

        public void Update(CategoriaDto dto, int idUsuario)
        {
            if (_repo.Get(dto.Id) == null)
                throw new ItemNotFoundException("La categoria no fue encontrada.");
            if (string.IsNullOrEmpty(dto.Nombre))
                throw new NullFieldException("La categoria debe tener un nombre.");
            if (dto.Presupuesto < 0)
                throw new MontoInvalidoException("El presupuesto de la categoria debe ser positivo.");
            if (ObtenerPorNombre(dto.Nombre) != null)
                throw new DuplicatedFieldException("Las categorias no deben estar duplicadas.");

            var categoria = new Categoria
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Presupuesto = dto.Presupuesto,
                IsActivo = dto.IsActivo,
                IdUsuario = idUsuario
            };

            _repo.Update(categoria);
        }

        public void Delete(int id, int idUsuario)
        {
            var gastosAsociados = gastoServices.ObtenerGastoPorcategoriaID(id);
            var categoriaExistente = _repo.Get(id);

            if (gastosAsociados.Any())
                throw new GastoAsociadoException("La categoria no puede ser eliminada. Contiene gastos asociados.");
            if (categoriaExistente.IdUsuario != idUsuario)
                throw new ItemNotFoundException("Su categoria no ha sido encontrada.");

            _repo.Delete(id);
        }

        public decimal VerificarPresupuesto(CategoriaDto dto, int idUsuario)
        {
            var gastosAsociados = gastoServices.ObtenerGastoPorcategoriaID(dto.Id);

            decimal gastoAcumulado = 0m;
            foreach(var g in gastosAsociados)
            {
                var gastoMes = gastoServices.ObtenerGastosMes(g.Fecha.Year, g.Fecha.Month, idUsuario);
                var gastosCategoria = gastoMes.Where(g => g.Categoria == dto.Nombre).Sum(g => g.Monto);
                gastoAcumulado = gastosCategoria;

                /*if (gastoAcumulado >= dto.Presupuesto * 0.5m)
                {
                    throw new AlertaPorcentaje("Alerta! Se ha superado el 50% de su presupuesto en esta categoria.");
                }

                if (gastoAcumulado >= dto.Presupuesto * 0.8m)
                {
                    throw new AlertaPorcentaje("Alerta! Se ha superado el 80% de su presupuesto en esta categoria.");
                }

                if (gastoAcumulado >= dto.Presupuesto)
                {
                    throw new AlertaPorcentaje("Alerta! Se ha superado el presupuesto en esta categoria.");
                }*/
            }
            return gastoAcumulado;
        }

        public decimal PorcentajePresupuesto(CategoriaDto dto, int idUsuario)
        {
            return (VerificarPresupuesto(dto, idUsuario) / dto.Presupuesto) * 100;
        }
    }
}
