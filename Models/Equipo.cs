using System.ComponentModel.DataAnnotations;
namespace LabTrack.Models;
public class Equipo {
 public int Id {get;set;}
 [Required,MaxLength(100)] public string Nombre {get;set;}="";
 [Required,MaxLength(30)] public string CodigoInventario {get;set;}="";
 [Required] public string Categoria {get;set;}="";
 public string Estado {get;set;}="Disponible";
 public List<Reserva> Reservas {get;set;}=new();
}
