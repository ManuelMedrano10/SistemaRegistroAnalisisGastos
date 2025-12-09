using Aplicacion.DTOs;
using Aplicacion.DTOs.Categoria;
using Aplicacion.Servicios;
using Dominio.Excepciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly CategoriaServices _categoriaServices;
        public CategoriaController(CategoriaServices categoriaServices)
        {
            _categoriaServices = categoriaServices;
        }
        private int ObtenerIdUsuario()
        {
            int idUsuario = int.Parse(User.FindFirst("Id").Value);

            return idUsuario;
        }

        [HttpGet]
        public IActionResult ObtenerCategorias()
        {
            var idUsuario = ObtenerIdUsuario();
            var categorias = _categoriaServices.GetAll(idUsuario);
            return Ok(categorias);
        }

        [HttpGet]
        public IActionResult ObtenerCategoriasActivas()
        {
            var idUsuario = ObtenerIdUsuario();
            var categorias = _categoriaServices.GetCategoriasActivas(idUsuario);
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public IActionResult Obtener(int id)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();
                var categoria = _categoriaServices.ObtenerPorId(id, idUsuario);

                return Ok(categoria);
            }
            catch(ItemNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Crear([FromBody] CategoriaCreateDto dto)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();
                _categoriaServices.Create(dto, idUsuario);

                return CreatedAtAction(nameof(Obtener), new { Id = _categoriaServices.ObtenerPorNombre(dto.Nombre).Id }, dto);
            }
            catch(NullFieldException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MontoInvalidoException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DuplicatedFieldException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] CategoriaDto dto)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();

                if (id != dto.Id)
                    return BadRequest(new { message = "Los IDs de las categorias no coinciden." });

                _categoriaServices.Update(dto, idUsuario);
                return NoContent();
            }
            catch (ItemNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (NullFieldException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MontoInvalidoException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DuplicatedFieldException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                var idUsuario = ObtenerIdUsuario();
                _categoriaServices.Delete(id, idUsuario);
                return NoContent();
            }
            catch (ItemNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (GastoAsociadoException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
