using AutoMapper;
using PeliculasAPIC.DTOs;
using PeliculasAPIC.Entidades;

namespace PeliculasAPIC.Helpers
{
    public class AutoMapperProfiles  : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());
            CreateMap<Actor, ActorPatchDTO>().ReverseMap();
        }
    }
}
