using System.ComponentModel.DataAnnotations;

namespace PeliculasAPIC.DTOs
{
    public class SalaDeCineDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }


    }
}
