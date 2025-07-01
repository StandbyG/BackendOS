using Microsoft.EntityFrameworkCore;
using OSControlSystem.Models;

namespace OSControlSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {}

        public DbSet<OrdenServicio> OrdenesServicio { get; set; }
        public DbSet<Etapa> Etapas { get; set; }
        public DbSet<Parada> Paradas { get; set; }
        public DbSet<MotivoParada> MotivosParada { get; set; }
        public DbSet<Reprogramacion> Reprogramaciones { get; set; }
        public DbSet<ListaDistribucion> ListasDistribucion { get; set; }    
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrdenServicio>()
                .HasIndex(o => o.Codigo)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}