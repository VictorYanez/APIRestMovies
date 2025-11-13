using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PeliculasAPIC.DTOs;
using PeliculasAPIC.Entidades;

namespace PeliculasAPIC.Controllers
{
    [ApiController]
    [Route("api/SalasDeCine")]
    public class SalasDeCineController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public SalasDeCineController(ApplicationDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<SalaDeCineDTO>>> Get()
        {
            return await Get<SalaDeCine, SalaDeCineDTO>();
        }

        [HttpGet("{id:int}", Name = "obtenerSalaDeCine")]
        public async Task<ActionResult<SalaDeCineDTO>> Get(int id)
        {
            return await Get<SalaDeCine, SalaDeCineDTO>(id);
        }     

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalaDeCineCreacionDTO salaDeCineCreacionDTO)
        {
            return await
             Post<SalaDeCineCreacionDTO, SalaDeCine, SalaDeCineDTO>(salaDeCineCreacionDTO, "obtenerSalaDeCine");
        }

        [HttpPost("{id}/AgregarPelicula/{peliculaId}")]
        public async Task<ActionResult> AgregarPelicula(int id, int peliculaId)
        {
            var peliculaSalaDeCine = new PeliculasSalasDeCine() 
            { PeliculaId = peliculaId, SalaDeCineId = id };
            context.Add(peliculaSalaDeCine);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] SalaDeCineCreacionDTO salaDeCineCreacionDTO)
        {
            return await Put<SalaDeCineCreacionDTO, SalaDeCine>(id, salaDeCineCreacionDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaDeCine>(id);
        }

    }
}
