namespace PeliculasAPIC.Entidades
{
    public class PeliculasGeneros
    {
        public int GeneroId { get; set; }
        public int PeliculaId { get; set; }

        // Propiedades de navegación
        public Genero Genero { get; set; }
        public Pelicula Pelicula { get; set; }
    }
}
