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
            try
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
            catch(NullFieldException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(EmailAlreadyInUseException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login(UsuarioLoginDto dto)
        {
            try
            {
                var resultado = _authServices.Login(dto);
                return Ok(new { token = resultado });
            }
            catch(ItemNotFoundException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch(InvalidLoginException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPut("actualizarNombre/{email}")]
        public IActionResult ActualizarNombre(string email, string nombre)
        {
            try
            {
                _authServices.ActualizarNombre(email, nombre);
                return NoContent();
            }
            catch(NullFieldException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("actualizarClave/{email}")]
        public IActionResult ActualizarClave(string email, string claveActual, string confirmarClave, string nuevaClave)
        {
            try
            {
                _authServices.ActualizarClave(email, claveActual, confirmarClave, nuevaClave);
                return NoContent();
            }
            catch (NullFieldException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ItemNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
