namespace PeliculasAPIC.Entidades
{
    public class PeliculasActores
    {
        public int ActorId { get; set; }
        public int PeliculaId { get; set; }
        public string Personaje { get; set; }
        public int Orden { get; set; }

        // Propiedades de navegación
        public Actor Actor { get; set; }
        public Pelicula Pelicula { get; set; }
    }
}
