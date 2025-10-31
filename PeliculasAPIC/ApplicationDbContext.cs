using Microsoft.EntityFrameworkCore;
using PeliculasAPIC.Entidades;

namespace PeliculasAPIC
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }   
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeliculasActores>()
                .HasKey(pa => new { pa.PeliculaId, pa.ActorId });
            modelBuilder.Entity<PeliculasGeneros>()
                .HasKey(pg => new { pg.PeliculaId, pg.GeneroId });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Genero> Generos { get; set; } = default!;
        public DbSet<Actor> Actores { get; set; } = default!;
        public DbSet<Pelicula> Peliculas { get; set; } = default!;
        public DbSet<PeliculasActores> PeliculasActores { get; set; } = default!;
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; } = default!;
    }
}
