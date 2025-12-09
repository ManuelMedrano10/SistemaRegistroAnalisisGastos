using System.Net;
using System.Text.Json;
using Dominio.Excepciones;

namespace Presentacion.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public void Invocar(HttpContext contexto)
        {
            try
            {
                _next(contexto);
            }
            catch(Exception ex)
            {
                var respuesta = contexto.Response;
                respuesta.ContentType = "application/json";

                switch (ex)
                {
                    case ItemNotFoundException:
                        respuesta.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    case FormatoInvalidoException:
                    case MontoInvalidoException:
                    case NullFieldException:
                    case DuplicatedFieldException:
                    case GastoAsociadoException:
                    case EmailAlreadyInUseException:
                        respuesta.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case InvalidLoginException:
                        respuesta.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;

                    default:
                        respuesta.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var resultado = JsonSerializer.Serialize(new { message = ex.Message });
                respuesta.WriteAsync(resultado);
            }
        }
    }
}
