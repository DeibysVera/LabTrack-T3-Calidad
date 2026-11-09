using System.ComponentModel.DataAnnotations;
namespace LabTrack.ViewModels;
public class ReservaViewModel {
 [Range(1,int.MaxValue,ErrorMessage="Seleccione un equipo")] public int EquipoId {get;set;}
 [Required] public DateTime FechaInicio {get;set;}
 [Required] public DateTime FechaFin {get;set;}
}
