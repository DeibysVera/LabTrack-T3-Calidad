using LabTrack.Data;
using LabTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LabTrack.Controllers;
[Authorize(Roles="Administrador")] public class AdminController(ApplicationDbContext db,ReservaService service):Controller{
 public async Task<IActionResult> Index(){ViewBag.Pendientes=await db.Reservas.Include(x=>x.Usuario).Include(x=>x.Equipo).Where(x=>x.Estado=="Pendiente").OrderBy(x=>x.FechaInicio).ToListAsync();ViewBag.Activas=await db.Reservas.Include(x=>x.Usuario).Include(x=>x.Equipo).Where(x=>x.Estado=="Aprobada").OrderBy(x=>x.FechaInicio).ToListAsync();return View();}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Aprobar(int id){var e=await service.AprobarAsync(id);TempData[e==null?"Ok":"Error"]=e??"Reserva aprobada.";return RedirectToAction("Index");}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Rechazar(int id,string motivo){var e=await service.RechazarAsync(id,motivo);TempData[e==null?"Ok":"Error"]=e??"Reserva rechazada.";return RedirectToAction("Index");}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Devolver(int id){var e=await service.DevolverAsync(id);TempData[e==null?"Ok":"Error"]=e??"Devolución registrada.";return RedirectToAction("Index");}
}
