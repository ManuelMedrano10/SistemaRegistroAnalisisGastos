using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.DTOs;
using Dominio.Entidades;

namespace Aplicacion.Servicios
{
    public class ReporteMensual
    {
        private readonly GastoServices gastoServices;
        private readonly CategoriaServices categoriaServices;

        public ReporteMensual(GastoServices gastoServices, CategoriaServices categoriaServices)
        {
            this.gastoServices = gastoServices;
            this.categoriaServices = categoriaServices;
        }

        public ResumenMensual GenerarResumen(int año, int mes, int idUsuario)
        {
            var gastos = gastoServices.ObtenerGastosMes(año, mes, idUsuario);
            var categorias = categoriaServices.GetAll(idUsuario);
            var totalGastado = gastos.Sum(g => g.Monto);
            var gastosUltimoMes = 0m;

            var resumen = new ResumenMensual(año, mes, totalGastado);

            foreach (var categoria in categorias)
            {
                var totalCategoria = categoriaServices.VerificarPresupuesto(categoria, idUsuario);
                if (totalCategoria > 0)
                {
                    resumen.TotalPorCategoria[categoria.Nombre] = totalCategoria;
                    resumen.PorcentajesPorCategoria[categoria.Nombre] = resumen.TotalGastado > 0
                        ? (totalCategoria / resumen.TotalGastado) * 100 : 0;
                    resumen.ComparacionMes[categoria.Nombre] = gastosUltimoMes = totalCategoria - gastosUltimoMes;
                }
            }

            return resumen;
        }
    }
}
