using PeliculasAPIC.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPIC.DTOs
{
    public class PeliculaPatchDTO
    {
        //public int Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }

    }
}
