using LabTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace LabTrack.Data;
public static class DemoSeed {
 public static async Task SeedAsync(ApplicationDbContext db,IPasswordHasher<Usuario> hasher){
  if(!await db.Usuarios.AnyAsync()){
   foreach(var (codigo,nombre,rol,clave) in new[]{("EST001","Estudiante de prueba","Estudiante","Est12345!"),("ADMIN001","Encargado de laboratorio","Administrador","Admin12345!")}){
    var u=new Usuario{CodigoEstudiante=codigo,Nombre=nombre,Rol=rol};u.PasswordHash=hasher.HashPassword(u,clave);db.Usuarios.Add(u);
   }
  }
  if(!await db.Equipos.AnyAsync())db.Equipos.AddRange(
   new Equipo{Nombre="Laptop Dell",CodigoInventario="COMP-001",Categoria="Cómputo"},
   new Equipo{Nombre="Osciloscopio",CodigoInventario="ELEC-001",Categoria="Electrónica"},
   new Equipo{Nombre="Router Cisco",CodigoInventario="RED-001",Categoria="Redes"},
   new Equipo{Nombre="Switch Cisco",CodigoInventario="RED-002",Categoria="Redes"});
  await db.SaveChangesAsync();
 }
}
