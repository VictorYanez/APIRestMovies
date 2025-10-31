using Microsoft.EntityFrameworkCore;
using PeliculasAPIC.Entidades;

namespace PeliculasAPIC
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }   
        public DbSet<Genero> Generos { get; set; } = default!;
        public DbSet<Actor> Actores { get; set; } = default!;
        public DbSet<Pelicula> Peliculas { get; set; } = default!;
    }
}
