using Microsoft.EntityFrameworkCore;

namespace PeliculasAPIC.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext, 
            IQueryable<T> queryable, int cantidadRegistrosPorPagina)
        {
            double cantidadTotalDeRegistros = await queryable.CountAsync();
            double cantidadTotalDePaginas = Math.Ceiling(cantidadTotalDeRegistros / cantidadRegistrosPorPagina);
            httpContext.Response.Headers.Append("cantidadTotalDePaginas", cantidadTotalDePaginas.ToString());
        }
    }
}
