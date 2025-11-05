using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPIC.DTOs;
using PeliculasAPIC.Entidades;
using PeliculasAPIC.Servicios;

namespace PeliculasAPIC.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<PeliculasController> _logger;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string _contenedor = "Peliculas";

        public PeliculasController(
            ApplicationDbContext context, 
            IMapper mapper,
             ILogger<PeliculasController> logger,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this._logger = logger;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<PeliculaDTO>>> Get()
        {
            var peliculas = await context.Peliculas.ToListAsync();
            var dtos = mapper.Map<List<PeliculaDTO>>(peliculas);
            return dtos;
        }

        [HttpGet("{id:int}", Name = "obtenerPelicula")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            if (pelicula == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<PeliculaDTO>(pelicula);
            return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido, extension,
                        _contenedor, peliculaCreacionDTO.Poster.ContentType);

                }

            }

            AsignarOrdenActores(pelicula);
            context.Add(pelicula);
            await context.SaveChangesAsync();
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);

            return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.Id }, peliculaDTO);

        }
        private void AsignarOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActores != null)
            {
                for (int i = 0; i < pelicula.PeliculasActores.Count; i++)
                {
                    // Asegurar que la FK esté establecida
                    if (pelicula.PeliculasActores[i].PeliculaId == 0 && pelicula.Id > 0)
                    {
                        pelicula.PeliculasActores[i].PeliculaId = pelicula.Id;
                    }
                    // Asignar orden comenzando desde 1
                    pelicula.PeliculasActores[i].Orden = i + 1;
                }
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            try
            {
                _logger.LogInformation($"🔵 INICIANDO PUT actualización de película ID: {id}");

                // 1. Verificar que la película existe
                var peliculaExiste = await context.Peliculas
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == id);

                if (!peliculaExiste)
                {
                    _logger.LogWarning($"Película con ID {id} no encontrada");
                    return NotFound();
                }

                _logger.LogInformation($"Película {id} encontrada, procediendo con actualización");

                // 2. Actualizar propiedades básicas usando enfoque directo
                var peliculaActual = await context.Peliculas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                var peliculaActualizada = new Pelicula
                {
                    Id = id,
                    Titulo = peliculaCreacionDTO.Titulo,
                    EnCines = peliculaCreacionDTO.EnCines,
                    FechaEstreno = peliculaCreacionDTO.FechaEstreno,
                    Poster = peliculaActual.Poster
                };

                _logger.LogInformation("Propiedades básicas mapeadas");

                // 3. Manejar el poster
                if (peliculaCreacionDTO.Poster != null)
                {
                    _logger.LogInformation("Procesando nuevo poster");
                    using (var memoryStream = new MemoryStream())
                    {
                        await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                        var contenido = memoryStream.ToArray();
                        var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                        peliculaActualizada.Poster = await almacenadorArchivos.EditarArchivo(
                            contenido, extension, _contenedor, peliculaActual.Poster,
                            peliculaCreacionDTO.Poster.ContentType);
                    }
                    _logger.LogInformation("Poster actualizado exitosamente");
                }

                // 4. Actualizar la película primero
                _logger.LogInformation("Actualizando entidad Película");
                context.Peliculas.Update(peliculaActualizada);
                await context.SaveChangesAsync();
                _logger.LogInformation("Película actualizada en BD");

                // 5. Actualizar relaciones por separado
                _logger.LogInformation("Iniciando actualización de relaciones");
                await ActualizarRelacionesPelicula(id, peliculaCreacionDTO);
                _logger.LogInformation("Todas las relaciones actualizadas exitosamente");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error crítico al actualizar película {id}: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }

                return StatusCode(500, $"Error interno detallado: {ex.Message}");
            }
        }

        private async Task ActualizarRelacionesPelicula(int peliculaId, PeliculaCreacionDTO peliculaCreacionDTO)
        {
            try
            {
                _logger.LogInformation($"Eliminando relaciones existentes para película {peliculaId}");

                // Eliminar relaciones existentes
                var actoresExistentes = await context.PeliculasActores
                    .Where(pa => pa.PeliculaId == peliculaId)
                    .ToListAsync();

                var generosExistentes = await context.PeliculasGeneros
                    .Where(pg => pg.PeliculaId == peliculaId)
                    .ToListAsync();

                _logger.LogInformation($"Encontrados {actoresExistentes.Count} actores y {generosExistentes.Count} géneros existentes");

                if (actoresExistentes.Any())
                {
                    context.PeliculasActores.RemoveRange(actoresExistentes);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Actores existentes eliminados");
                }

                if (generosExistentes.Any())
                {
                    context.PeliculasGeneros.RemoveRange(generosExistentes);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Géneros existentes eliminados");
                }

                // Agregar nuevos géneros
                if (peliculaCreacionDTO.GenerosIDs != null && peliculaCreacionDTO.GenerosIDs.Any())
                {
                    _logger.LogInformation($"Agregando {peliculaCreacionDTO.GenerosIDs.Count} nuevos géneros");

                    foreach (var generoId in peliculaCreacionDTO.GenerosIDs)
                    {
                        // Verificar que el género existe
                        var generoExiste = await context.Generos.AnyAsync(g => g.Id == generoId);
                        if (!generoExiste)
                        {
                            _logger.LogWarning($"Género con ID {generoId} no existe, omitiendo");
                            continue;
                        }

                        context.PeliculasGeneros.Add(new PeliculasGeneros
                        {
                            GeneroId = generoId,
                            PeliculaId = peliculaId
                        });
                    }
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Géneros agregados exitosamente");
                }

                // Agregar nuevos actores
                if (peliculaCreacionDTO.Actores != null && peliculaCreacionDTO.Actores.Any())
                {
                    _logger.LogInformation($"Agregando {peliculaCreacionDTO.Actores.Count} nuevos actores");

                    for (int i = 0; i < peliculaCreacionDTO.Actores.Count; i++)
                    {
                        var actorDTO = peliculaCreacionDTO.Actores[i];

                        // Verificar que el actor existe
                        var actorExiste = await context.Actores.AnyAsync(a => a.Id == actorDTO.ActorId);
                        if (!actorExiste)
                        {
                            _logger.LogWarning($"Actor con ID {actorDTO.ActorId} no existe, omitiendo");
                            continue;
                        }

                        context.PeliculasActores.Add(new PeliculasActores
                        {
                            ActorId = actorDTO.ActorId,
                            PeliculaId = peliculaId,
                            Personaje = actorDTO.Personaje ?? "",
                            Orden = i + 1
                        });
                    }
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Actores agregados exitosamente");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en ActualizarRelacionesPelicula: {ex.Message}");
                throw;
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument)
        {

            if (patchDocument == null)
            {
                return NotFound();
            }

            var entidadDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            if (entidadDB == null)
            {
                return NotFound();
            }

            var entidadDTO = mapper.Map<PeliculaPatchDTO>(entidadDB);

            patchDocument.ApplyTo(entidadDTO, ModelState);
            var isValid = TryValidateModel(entidadDTO);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(entidadDTO, entidadDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Peliculas.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new Pelicula() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();

        }

    }
}
