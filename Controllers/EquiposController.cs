using LabTrack.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LabTrack.Controllers;
[Authorize] public class EquiposController(ApplicationDbContext db):Controller{
 public async Task<IActionResult> Index(string? categoria){
  var validas=new[]{"Cómputo","Electrónica","Redes"};
  ViewBag.Categoria=categoria;
  var query=db.Equipos.AsNoTracking().AsQueryable();
  if(!string.IsNullOrEmpty(categoria))query=validas.Contains(categoria)?query.Where(x=>x.Categoria==categoria):query.Where(x=>false);
  return View(await query.OrderBy(x=>x.Nombre).ToListAsync());
 }
}
