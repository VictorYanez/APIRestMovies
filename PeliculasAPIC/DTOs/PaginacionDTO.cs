using Newtonsoft.Json.Linq;

namespace PeliculasAPIC.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        public int cantidadRegistrosPorPagina = 11;
        private readonly int cantidadMaximaRegistrosPorPagina = 50;
        public int CantidadRegistrosPorPagina
        {
                get => cantidadRegistrosPorPagina;
                set => cantidadRegistrosPorPagina = 
                    (value > cantidadMaximaRegistrosPorPagina) ? 
                      cantidadMaximaRegistrosPorPagina : value;
        }

    }
}
