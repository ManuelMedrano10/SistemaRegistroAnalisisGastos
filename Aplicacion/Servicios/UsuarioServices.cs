using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.DTOs.Usuario;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Excepciones;

namespace Aplicacion.Servicios
{
    public class UsuarioServices
    {
        private readonly IRepository<Usuario> _repo;
        public UsuarioServices(IRepository<Usuario> repo)
        {
            _repo = repo;
        }

        public Usuario ObtenerUsuarioPorEmail(string email)
        {
            return _repo.GetAll().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public void Create(UsuarioRegisterDto dto)
        {
            if (string.IsNullOrEmpty(dto.Nombre))
                throw new NullFieldException("Se necesita un nombre para registrarse.");
            if (string.IsNullOrEmpty(dto.Email))
                throw new NullFieldException("Se necesita un correo para registrarse.");
            if (string.IsNullOrEmpty(dto.Clave))
                throw new NullFieldException("Se necesita una contraseña para registrarse.");
            if (ObtenerUsuarioPorEmail(dto.Email) != null)
                throw new EmailAlreadyInUseException("El correo utilizado ya existe. Por favor, utilice otro correo o inicie sesion.");

            dto.Clave = HashClave(dto.Clave);
            _repo.Add(new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email.ToLower(),
                Clave = dto.Clave
            });
        }

        public void Update(string email, string claveActual, UsuarioUpdateDto dto)
        {
            var usuarios = _repo.GetAll();
            var usuarioExistente = usuarios.FirstOrDefault(u => u.Email == email);

            if (!BCrypt.Net.BCrypt.Verify(claveActual, usuarioExistente.Clave))
                throw new ItemNotFoundException("Su clave no coincide. Intentelo de nuevo.");
            if (string.IsNullOrEmpty(dto.Nombre))
                throw new NullFieldException("Se necesita un nombre para registrarse.");
            if (string.IsNullOrEmpty(dto.Clave))
                throw new NullFieldException("Se necesita una contraseña para registrarse.");

            _repo.Update(new Usuario
            {
                Nombre = dto.Nombre,
                Clave = HashClave(dto.Clave)
            });
        }

        public void Delete(string correo)
        {
            var usuario = ObtenerUsuarioPorEmail(correo);
            _repo.Delete(usuario.Id);
        }

        private string HashClave(string clave)
        {
            return BCrypt.Net.BCrypt.HashPassword(clave, 10);
        }
    }
}
