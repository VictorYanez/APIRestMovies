namespace PeliculasAPIC.DTOs
{
    public class ActorPeliculasCreacionDTO
    {
        public int ActorId { get; set; }
        public string Personaje { get; set; }

        // ✅ Esta línea es necesaria???
        //public int Orden { get; set; } 

    }
}
