using Aplicacion.DTOs.Usuario;
using Aplicacion.Servicios;
using Dominio.Excepciones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthServices _authServices;
        public AuthController(AuthServices authServices)
        {
            _authServices = authServices;
        }

        [HttpPost("register")]
        public IActionResult Registrar(UsuarioRegisterDto dto)
        {
            string clave = dto.Clave;
            _authServices.Registrar(dto);

            var token = _authServices.Login(new UsuarioLoginDto
            {
                Email = dto.Email,
                Clave = clave
            });
            return Ok(new { token });
        }

        [HttpPost("login")]
        public IActionResult Login(UsuarioLoginDto dto)
        {
            var resultado = _authServices.Login(dto);
            return Ok(new { token = resultado });
        }

        [HttpPut("actualizarNombre/{email}")]
        public IActionResult ActualizarNombre(string email, string nombre)
        {
            _authServices.ActualizarNombre(email, nombre);
            return NoContent();
        }

        [HttpPut("actualizarClave/{email}")]
        public IActionResult ActualizarClave(string email, string claveActual, string confirmarClave, string nuevaClave)
        {
            _authServices.ActualizarClave(email, claveActual, confirmarClave, nuevaClave);
            return NoContent();
        }
    }
}
