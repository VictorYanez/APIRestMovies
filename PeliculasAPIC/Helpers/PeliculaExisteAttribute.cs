using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace PeliculasAPIC.Helpers
{
    public class PeliculaExisteAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDbContext Dbcontext;

        public PeliculaExisteAttribute(ApplicationDbContext context)
        {
            this.Dbcontext = context;
        }

        // Implementación del filtro de recurso
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
            ResourceExecutionDelegate next)
        {
            // Obtener el valor de "peliculaId" de las rutas que viene de la URL en ReviewsController
            var peliculaIdObject = context.HttpContext.Request.RouteValues["peliculaId"];

            if (peliculaIdObject == null)
            {
                return;
            }

            var peliculaId = int.Parse(peliculaIdObject.ToString());

            var existePelicula = await Dbcontext.Peliculas.AnyAsync(x => x.Id == peliculaId);

            if (!existePelicula)
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                await next();
            }
        }
    }
}
