namespace PeliculasAPIC.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType);
        Task BorrarArchivo(string ruta, string contenedor);

        Task<string> EditarArchivo(byte[] contenido, 
            string extension, string contenedor, string rutaActual, string contentType);
    }
}
