using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.ApiResponse
{
    public class ApiResponse<T>
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public T Data { get; set; }
        public List<string> Errores { get; set; } = new List<string>();

        public ApiResponse(T data, string mensaje = "Operacion exitosa.")
        {
            Exito = true;
            Mensaje = mensaje;
            Data = data;
        }

        public ApiResponse(string mensaje, bool exito = false, List<string> errores = null)
        {
            Exito = exito;
            Mensaje = mensaje;
            Data = default;
            if(errores != null)
            {
                Errores = errores;
            }
        }
    }
}
