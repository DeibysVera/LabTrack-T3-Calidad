using System.Security.Claims;
using LabTrack.Data;
using LabTrack.Models;
using LabTrack.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LabTrack.Controllers;
public class CuentaController(ApplicationDbContext db,IPasswordHasher<Usuario> hasher):Controller {
 [HttpGet,AllowAnonymous] public IActionResult Login()=>View(new LoginViewModel());
 [HttpPost,AllowAnonymous,ValidateAntiForgeryToken] public async Task<IActionResult> Login(LoginViewModel model){
  if(!ModelState.IsValid)return View(model);
  var user=await db.Usuarios.SingleOrDefaultAsync(x=>x.CodigoEstudiante==model.Codigo.Trim());
  if(user==null){ModelState.AddModelError("","Credenciales inválidas");return View(model);}
  var now=DateTime.UtcNow;
  if(user.BloqueadoHasta>now){ModelState.AddModelError("","Alerta de seguridad: cuenta bloqueada temporalmente. Intente más tarde.");return View(model);}
  if(user.BloqueadoHasta<=now){user.BloqueadoHasta=null;user.IntentosFallidos=0;}
  var result=hasher.VerifyHashedPassword(user,user.PasswordHash,model.Password);
  if(result==PasswordVerificationResult.Failed){
   user.IntentosFallidos++;
   if(user.IntentosFallidos>=3){user.BloqueadoHasta=now.AddMinutes(5);ModelState.AddModelError("","Alerta de seguridad: cuenta bloqueada temporalmente por 5 minutos.");}
   else ModelState.AddModelError("","Credenciales inválidas");
   await db.SaveChangesAsync();return View(model);
  }
  user.IntentosFallidos=0;user.BloqueadoHasta=null;
  if(result==PasswordVerificationResult.SuccessRehashNeeded)user.PasswordHash=hasher.HashPassword(user,model.Password);
  await db.SaveChangesAsync();
  var claims=new[]{new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),new Claim(ClaimTypes.Name,user.Nombre),new Claim(ClaimTypes.Role,user.Rol)};
  await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme)));
  return RedirectToAction("Index",user.Rol=="Administrador"?"Admin":"Equipos");
 }
 [HttpPost,Authorize,ValidateAntiForgeryToken] public async Task<IActionResult> Salir(){await HttpContext.SignOutAsync();return RedirectToAction("Login");}
 public IActionResult Denegado()=>View();
}
