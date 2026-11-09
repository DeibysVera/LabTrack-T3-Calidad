using System.ComponentModel.DataAnnotations;
namespace LabTrack.Models;
public class Usuario {
 public int Id {get;set;}
 [Required,MaxLength(20)] public string CodigoEstudiante {get;set;}="";
 [Required,MaxLength(100)] public string Nombre {get;set;}="";
 [Required] public string PasswordHash {get;set;}="";
 public string Rol {get;set;}="Estudiante";
 public int IntentosFallidos {get;set;}
 public DateTime? BloqueadoHasta {get;set;}
 public bool Penalizado {get;set;}
 public List<Reserva> Reservas {get;set;}=new();
}
