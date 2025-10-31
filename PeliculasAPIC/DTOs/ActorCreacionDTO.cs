using PeliculasAPIC.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPIC.DTOs
{
    public class ActorCreacionDTO
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }

        [PesoArchivoValidacion(PesoMaximoEnMB:4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Foto { get; set; }  //Archivo de imagen

    }
}
