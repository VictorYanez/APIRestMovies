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
                .HasKey(pa => new { pa.ActorId, pa.PeliculaId });

            modelBuilder.Entity<PeliculasActores>()
                .HasOne(pa => pa.Actor)
                .WithMany(a => a.PeliculasActores)
                .HasForeignKey(pa => pa.ActorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PeliculasActores>()
                .HasOne(pa => pa.Pelicula)
                .WithMany(p => p.PeliculasActores)
                .HasForeignKey(pa => pa.PeliculaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PeliculasGeneros>()
                .HasKey(pg => new { pg.GeneroId, pg.PeliculaId });

            modelBuilder.Entity<PeliculasGeneros>()
                .HasOne(pg => pg.Genero)
                .WithMany(g => g.PeliculasGeneros)
                .HasForeignKey(pg => pg.GeneroId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PeliculasGeneros>()
                .HasOne(pg => pg.Pelicula)
                .WithMany(p => p.PeliculasGeneros)
                .HasForeignKey(pg => pg.PeliculaId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Genero> Generos { get; set; } = default!;
        public DbSet<Actor> Actores { get; set; } = default!;
        public DbSet<Pelicula> Peliculas { get; set; } = default!;
        public DbSet<PeliculasActores> PeliculasActores { get; set; } = default!;
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; } = default!;

    }
}
