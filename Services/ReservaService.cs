using LabTrack.Data;
using LabTrack.Models;
using Microsoft.EntityFrameworkCore;
namespace LabTrack.Services;
public class ReservaService(ApplicationDbContext db){
 public static bool SeCruzan(DateTime aInicio,DateTime aFin,DateTime bInicio,DateTime bFin)=>aInicio<bFin && bInicio<aFin;
 public async Task<string?> SolicitarAsync(int usuarioId,int equipoId,DateTime inicio,DateTime fin){
  if(inicio>=fin||inicio<=DateTime.Now)return "Seleccione un intervalo futuro válido.";
  var usuario=await db.Usuarios.FindAsync(usuarioId);
  if(usuario==null)return "Usuario inexistente.";
  if(usuario.Penalizado)return "El estudiante está penalizado.";
  var equipo=await db.Equipos.FindAsync(equipoId);
  if(equipo==null)return "Equipo inexistente.";
  if(equipo.Estado=="Mantenimiento")return "Equipo en mantenimiento.";
  var reservas=await db.Reservas.Where(r=>(r.Estado=="Pendiente"||r.Estado=="Aprobada") && (r.EquipoId==equipoId||r.UsuarioId==usuarioId)).ToListAsync();
  if(reservas.Any(r=>r.EquipoId==equipoId && r.Estado=="Aprobada" && SeCruzan(inicio,fin,r.FechaInicio,r.FechaFin)))return "El equipo ya tiene una reserva aprobada en ese horario.";
  if(reservas.Any(r=>r.UsuarioId==usuarioId && SeCruzan(inicio,fin,r.FechaInicio,r.FechaFin)))return "No puede tener más de una reserva simultánea.";
  db.Reservas.Add(new Reserva{UsuarioId=usuarioId,EquipoId=equipoId,FechaInicio=inicio,FechaFin=fin});await db.SaveChangesAsync();return null;
 }
 public async Task<string?> AprobarAsync(int id){
  // Transacción serializable para comprobar nuevamente conflictos al aprobar.
  await using var tx=await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
  var r=await db.Reservas.Include(x=>x.Equipo).FirstOrDefaultAsync(x=>x.Id==id);
  if(r==null||r.Estado!="Pendiente")return "La solicitud no está pendiente.";
  var otros=await db.Reservas.Where(x=>x.Id!=id && x.Estado=="Aprobada" && (x.EquipoId==r.EquipoId||x.UsuarioId==r.UsuarioId)).ToListAsync();
  if(otros.Any(x=>SeCruzan(r.FechaInicio,r.FechaFin,x.FechaInicio,x.FechaFin)))return "Conflicto con una reserva aprobada; no se puede aprobar.";
  r.Estado="Aprobada";r.Equipo.Estado="Prestado";
  await db.SaveChangesAsync();await tx.CommitAsync();return null;
 }
 public async Task<string?> RechazarAsync(int id,string? motivo){
  if(string.IsNullOrWhiteSpace(motivo))return "El motivo de rechazo es obligatorio.";
  var r=await db.Reservas.FindAsync(id);
  if(r==null||r.Estado!="Pendiente")return "La solicitud no está pendiente.";
  r.Estado="Rechazada";r.MotivoRechazo=motivo.Trim()[..Math.Min(motivo.Trim().Length,300)];await db.SaveChangesAsync();return null;
 }
 public async Task<string?> DevolverAsync(int id){
  var r=await db.Reservas.Include(x=>x.Usuario).Include(x=>x.Equipo).FirstOrDefaultAsync(x=>x.Id==id);
  if(r==null||r.Estado!="Aprobada")return "No existe una reserva activa aprobada.";
  var ahora=DateTime.Now;r.FechaDevolucion=ahora;r.Estado="Devuelta";
  if(ahora>r.FechaFin)r.Usuario.Penalizado=true;
  var otraActiva=await db.Reservas.AnyAsync(x=>x.Id!=id&&x.EquipoId==r.EquipoId&&x.Estado=="Aprobada"&&x.FechaInicio<=ahora&&x.FechaFin>ahora);
  r.Equipo.Estado=otraActiva?"Prestado":"Disponible";
  await db.SaveChangesAsync();return null;
 }
}
