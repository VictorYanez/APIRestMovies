using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPIC.DTOs;
using PeliculasAPIC.Entidades;

namespace PeliculasAPIC.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();

            // Mapear Reviews
            CreateMap<Review, ReviewDTO>()
                .ForMember(x => x.NombreUsuario, x => x.MapFrom(y => y.Usuario.UserName));
            CreateMap<ReviewDTO, Review>();
            CreateMap<ReviewCreacionDTO, Review>();

            // Mapear Usuarios
            CreateMap<IdentityUser, UsuarioDTO>();

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());
            CreateMap<Actor, ActorPatchDTO>().ReverseMap();

            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            //CreateMap<PeliculaCreacionDTO, Pelicula>()
            //    .ForMember(x => x.Poster, options => options.Ignore())
            //    .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneros))
            //    .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculasActores));

            // ⚠️ TEMPORALMENTE: No mapear relaciones automáticamente
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.PeliculasGeneros, options => options.Ignore())
                .ForMember(x => x.PeliculasActores, options => options.Ignore())
                .ForMember(x => x.Id, options => options.Ignore());

            CreateMap<Pelicula, PeliculaDetallesDTO>()
                .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculasActores));

            CreateMap<Pelicula, PeliculaDetallesDTO>()
                          .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculasGeneros))
                          .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculasActores));

            CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();

            // Salas de cine y geolocalización
            CreateMap<SalaDeCine, SalaDeCineDTO>()
                .ForMember(x => x.Latitud, options => options.MapFrom(y => y.Ubicacion.Y))
                .ForMember(x => x.Longitud, options => options.MapFrom(y => y.Ubicacion.X));

            CreateMap<SalaDeCineDTO, SalaDeCine>()
                .ForMember(x => x.Ubicacion, options => options.MapFrom(y =>
                    geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));

            CreateMap<SalaDeCineCreacionDTO, SalaDeCine>()
                .ForMember(x => x.Ubicacion, options => options.MapFrom(y =>
                    geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))));

        }

        private List<ActorPeliculaDetalleDTO> MapPeliculasActores(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<ActorPeliculaDetalleDTO>();
            if (pelicula.PeliculasActores == null) { return resultado; }
            foreach (var actorPelicula in pelicula.PeliculasActores)
            {
                resultado.Add(new ActorPeliculaDetalleDTO
                {
                    ActorId = actorPelicula.ActorId,
                    Personaje = actorPelicula.Personaje,
                    NombrePersona = actorPelicula.Actor.Nombre
                });
            }

            return resultado;
        }

        private List<GeneroDTO> MapPeliculasGeneros(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<GeneroDTO>();
            if (pelicula.PeliculasGeneros == null) { return resultado; }
            foreach (var generoPelicula in pelicula.PeliculasGeneros)
            {
                resultado.Add(new GeneroDTO() { Id = generoPelicula.GeneroId, Nombre = generoPelicula.Genero.Nombre });
            }

            return resultado;
        }

        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if (peliculaCreacionDTO.GenerosIDs == null) { return resultado; }
            foreach (var id in peliculaCreacionDTO.GenerosIDs)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }

            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if (peliculaCreacionDTO.Actores == null) { return resultado; }

            foreach (var actor in peliculaCreacionDTO.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.ActorId, Personaje = actor.Personaje });
            }

            return resultado;
        }
     
    }
}
