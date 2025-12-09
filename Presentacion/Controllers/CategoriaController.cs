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

        [HttpGet("activas")]
        public IActionResult ObtenerCategoriasActivas()
        {
            var idUsuario = ObtenerIdUsuario();
            var categorias = _categoriaServices.GetCategoriasActivas(idUsuario);
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public IActionResult Obtener(int id)
        {
            var idUsuario = ObtenerIdUsuario();
            var categoria = _categoriaServices.ObtenerPorId(id, idUsuario);

            return Ok(categoria);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] CategoriaCreateDto dto)
        {
            var idUsuario = ObtenerIdUsuario();
            _categoriaServices.Create(dto, idUsuario);

            return CreatedAtAction(nameof(Obtener), new { Id = _categoriaServices.ObtenerPorNombre(dto.Nombre).Id }, dto);
        }

        [HttpPut("{id}")]
        public IActionResult Actualizar(int id, [FromBody] CategoriaDto dto)
        {
            var idUsuario = ObtenerIdUsuario();

            if (id != dto.Id)
                return BadRequest(new { message = "Los IDs de las categorias no coinciden." });

            _categoriaServices.Update(dto, idUsuario);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            var idUsuario = ObtenerIdUsuario();
            _categoriaServices.Delete(id, idUsuario);
            return NoContent();
        }
    }
}
