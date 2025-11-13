using System.ComponentModel.DataAnnotations;

namespace PeliculasAPIC.Entidades
{
    public class Actor : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Foto { get; set; }  //URL de imagen

        // Campos de navegación
        public List<PeliculasActores> PeliculasActores { get; set; }

    }
}
