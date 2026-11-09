using System.Security.Claims;
using LabTrack.Data;
using LabTrack.Services;
using LabTrack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LabTrack.Controllers;
[Authorize(Roles="Estudiante")] public class ReservasController(ApplicationDbContext db,ReservaService service):Controller{
 private int UsuarioId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 public async Task<IActionResult> Index()=>View(await db.Reservas.Include(x=>x.Equipo).Where(x=>x.UsuarioId==UsuarioId).OrderByDescending(x=>x.FechaInicio).ToListAsync());
 [HttpGet] public async Task<IActionResult> Crear(int? equipoId){ViewBag.Equipos=await db.Equipos.OrderBy(x=>x.Nombre).ToListAsync();return View(new ReservaViewModel{EquipoId=equipoId??0,FechaInicio=DateTime.Now.AddDays(1),FechaFin=DateTime.Now.AddDays(1).AddHours(1)});}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Crear(ReservaViewModel model){
  if(ModelState.IsValid){var error=await service.SolicitarAsync(UsuarioId,model.EquipoId,model.FechaInicio,model.FechaFin);if(error==null){TempData["Ok"]="Solicitud registrada.";return RedirectToAction("Index");}ModelState.AddModelError("",error);}
  ViewBag.Equipos=await db.Equipos.OrderBy(x=>x.Nombre).ToListAsync();return View(model);
 }
}
