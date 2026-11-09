using LabTrack.Models;
using Microsoft.EntityFrameworkCore;
namespace LabTrack.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):DbContext(options) {
 public DbSet<Usuario> Usuarios=>Set<Usuario>();
 public DbSet<Equipo> Equipos=>Set<Equipo>();
 public DbSet<Reserva> Reservas=>Set<Reserva>();
 protected override void OnModelCreating(ModelBuilder b) {
  b.Entity<Usuario>().HasIndex(x=>x.CodigoEstudiante).IsUnique();
  b.Entity<Equipo>().HasIndex(x=>x.CodigoInventario).IsUnique();
  b.Entity<Reserva>().HasOne(x=>x.Usuario).WithMany(x=>x.Reservas).HasForeignKey(x=>x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Reserva>().HasOne(x=>x.Equipo).WithMany(x=>x.Reservas).HasForeignKey(x=>x.EquipoId).OnDelete(DeleteBehavior.Restrict);
 }
}
