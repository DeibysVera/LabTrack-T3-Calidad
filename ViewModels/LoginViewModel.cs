using System.ComponentModel.DataAnnotations;
namespace LabTrack.ViewModels;
public class LoginViewModel {
 [Required(ErrorMessage="Ingrese su código")] public string Codigo {get;set;}="";
 [Required(ErrorMessage="Ingrese su contraseña"),DataType(DataType.Password)] public string Password {get;set;}="";
}
