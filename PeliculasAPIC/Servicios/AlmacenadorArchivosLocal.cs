
namespace PeliculasAPIC.Servicios
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos

    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType)
        {
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, contenedor);
            
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string rutaCrear = Path.Combine(folder, nombreArchivo);
            await File.WriteAllBytesAsync(rutaCrear, contenido);

            var urlActual = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var rutaParaBD = Path.Combine(urlActual, contenedor, nombreArchivo).Replace("\\", "/");
            return rutaParaBD;

        }
        public  Task BorrarArchivo(string ruta, string contenedor)
        {
            if (ruta != null)
            {
                var nombreArchivo = Path.GetFileName(ruta);
                string directorioArchivo = Path.Combine(env.WebRootPath, contenedor);
                string rutaArchivo = Path.Combine(directorioArchivo, nombreArchivo);
                if (File.Exists(rutaArchivo))
                {
                    File.Delete(rutaArchivo);
                }  
            }

            return  Task.FromResult(0);

        }

        public async  Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string rutaActual, string contentType)
        {
            await BorrarArchivo(rutaActual, contenedor);
            return await GuardarArchivo(contenido, extension, contenedor, contentType);
        }

    }
}
