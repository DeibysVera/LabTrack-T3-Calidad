using System.ComponentModel.DataAnnotations;
namespace LabTrack.Models;
public class Reserva {
 public int Id {get;set;}
 public int UsuarioId {get;set;}
 public Usuario Usuario {get;set;}=null!;
 public int EquipoId {get;set;}
 public Equipo Equipo {get;set;}=null!;
 public DateTime FechaInicio {get;set;}
 public DateTime FechaFin {get;set;}
 public string Estado {get;set;}="Pendiente";
 [MaxLength(300)] public string? MotivoRechazo {get;set;}
 public DateTime? FechaDevolucion {get;set;}
}
