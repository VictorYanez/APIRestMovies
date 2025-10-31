using System.ComponentModel.DataAnnotations;

namespace PeliculasAPIC.Validaciones
{
    public class PesoArchivoValidacion : ValidationAttribute
    {
        private readonly int pesoMaximoEnMB;

        public PesoArchivoValidacion(int PesoMaximoEnMB)
        {
            pesoMaximoEnMB = PesoMaximoEnMB;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            if (formFile.Length > pesoMaximoEnMB * 1024 * 1024)
            {
                return new ValidationResult($"El peso del archivo no debe exceder {pesoMaximoEnMB} MB.");
            }

            return ValidationResult.Success;
        }
    }
}
