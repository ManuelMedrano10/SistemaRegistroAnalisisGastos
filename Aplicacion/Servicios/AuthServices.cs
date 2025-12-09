using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.DTOs.Usuario;
using Aplicacion.Utils;
using Dominio.Excepciones;
using Microsoft.Extensions.Configuration;

namespace Aplicacion.Servicios
{
    public class AuthServices
    {
        private readonly UsuarioServices _usuarioServices;
        private readonly IConfiguration _config;
        public AuthServices(UsuarioServices usuarioServices, IConfiguration config)
        {
            _usuarioServices = usuarioServices;
            _config = config;
        }

        public string Login(UsuarioLoginDto dto)
        {
            var usuario = _usuarioServices.ObtenerUsuarioPorEmail(dto.Email);
            if (usuario == null)
                throw new ItemNotFoundException("Su correo ingresado es incorrecto.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Clave, usuario.Clave))
                throw new InvalidLoginException("Su contraseña es invalida.");

            return JwtHelper.GenerarToken(usuario.Id, usuario.Nombre, usuario.Email, _config);
        }

        public bool Registrar(UsuarioRegisterDto dto)
        {
            _usuarioServices.Create(dto);
            return true;
        }

        public void ActualizarNombre(string email, string nuevoNombre)
        {
            _usuarioServices.Update(email, nuevoNombre);
        }

        public void ActualizarClave(string email, string claveActual, string confirmarClave, string nuevaClave)
        {
            _usuarioServices.Update(email, claveActual, confirmarClave, nuevaClave);
        }
    }
}
