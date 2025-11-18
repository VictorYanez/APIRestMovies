using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPIC.DTOs;
using PeliculasAPIC.Entidades;
using PeliculasAPIC.Helpers;

namespace PeliculasAPIC.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(ApplicationDbContext context, IMapper mapper )
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class
        {
            var entidades = await context.Set<TEntidad>().AsNoTracking().ToListAsync();
            var dtos = mapper.Map<List<TDTO>>(entidades);
            return dtos;
        }

        protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where TEntidad : class, IId
        {
            var entidad = await context.Set<TEntidad>().AsNoTracking()
                .FirstOrDefaultAsync( x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }
            var dto = mapper.Map<TDTO>(entidad);
            return dto;

        }

        // ✅ NUEVO MÉTODO - Sobrecarga para PaginacionDTO
        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginacionDTO paginacionDTO) where TEntidad : class
        {
            var queryable = context.Set<TEntidad>().AsNoTracking().AsQueryable();
            var entidades = await queryable
                .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.CantidadRegistrosPorPagina)
                .Take(paginacionDTO.CantidadRegistrosPorPagina)
                .ToListAsync();
            var dtos = mapper.Map<List<TDTO>>(entidades);
            return dtos;
        }

        // ✅ NUEVO MÉTODO - Sobrecarga para PaginacionDTO
        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginacionDTO paginacionDTO, 
            IQueryable<TEntidad> queryable) where TEntidad : class
        {
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable, 
                paginacionDTO.CantidadRegistrosPorPagina);
            var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();
            var dtos = mapper.Map<List<TDTO>>(entidades);
            return dtos;
        }
        protected async Task<ActionResult> Post<TCreacion, TEntidad, TLectura>
               (TCreacion creacionDTO, string nombreRuta) where TEntidad : class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacionDTO);
            context.Add(entidad);
            await context.SaveChangesAsync();
            var generoDTO = mapper.Map<TLectura>(entidad);


            return NoContent();
        }

        protected async Task<ActionResult> Put<TCreacion, TEntidad>
           (int id, TCreacion creacionDTO) where TEntidad : class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacionDTO);
            entidad.Id = id;
            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }
        protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad : class, IId, new()
        {
            var existe = await context.Set<TEntidad>().AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new TEntidad() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntidad, TDTO>(int id, JsonPatchDocument<TDTO> patchDocument)
            where TDTO : class
            where TEntidad : class, IId
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entidadDB = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);

            if (entidadDB == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<TDTO>(entidadDB);

            patchDocument.ApplyTo(dto, ModelState);

            var isValid = TryValidateModel(dto);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(dto, entidadDB);

            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
