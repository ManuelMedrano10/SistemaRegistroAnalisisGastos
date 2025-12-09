using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.DTOs.MetodoPago;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Excepciones;

namespace Aplicacion.Servicios
{
    public class MetodoPagoServices
    {
        private readonly IRepository<MetodoPago> _repo;
        public MetodoPagoServices(IRepository<MetodoPago> repo)
        {
            _repo = repo;
        }

        public IEnumerable<MetodoPagoDto> ObtenerMetodosPago(int idUsuario)
        {
            return _repo.GetAll().Select(m => new MetodoPagoDto
            {
                Id = m.Id,
                Nombre = m.Nombre
            }).ToList().Where(m => _repo.Get(m.Id).IdUsuario == idUsuario);
        }

        public MetodoPago ObtenerPorNombre(string nombre)
        {
            var metodosP = _repo.GetAll();
            return metodosP.FirstOrDefault(m => m.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }

        public MetodoPagoDto ObtenerPorId(int id, int idUsuario)
        {
            var metodoP = _repo.Get(id);

            if (metodoP.IdUsuario == idUsuario)
            {
                return new MetodoPagoDto
                {
                    Id = metodoP.Id,
                    Nombre = metodoP.Nombre
                };
            }
            else
            {
                throw new ItemNotFoundException("Su metodo de pago no se encontrado.");
            }
        }

        public void Create(MetodoPagoCreateDto dto, int idUsuario)
        {
            var metodoDuplicado = ObtenerPorNombre(dto.Nombre);
            if (metodoDuplicado != null)
                throw new DuplicatedFieldException("El metodo de pago insertado ya existe.");
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new NullFieldException("El metodo de pago necesita un nombre");

            var metodoP = new MetodoPago
            {
                Nombre = dto.Nombre,
                IdUsuario = idUsuario
            };
            _repo.Add(metodoP);
        }

        public void Update(MetodoPagoDto dto, int idUsuario)
        {
            var metodoDuplicado = ObtenerPorNombre(dto.Nombre);
            if (metodoDuplicado != null)
                throw new DuplicatedFieldException("El metodo de pago insertado ya existe.");
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new NullFieldException("El metodo de pago necesita un nombre");

            var metodoP = new MetodoPago
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                IdUsuario = idUsuario
            };
            _repo.Update(metodoP);
        }
        public void Delete(int id, int idUsuario)
        {
            var metodoExistente = _repo.Get(id);
            if (metodoExistente.IdUsuario != idUsuario)
                throw new ItemNotFoundException("Su metodo de pago no ha sido encontrado.");
            _repo.Delete(id);
        }
    }
}
