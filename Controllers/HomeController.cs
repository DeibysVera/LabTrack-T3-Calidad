using Microsoft.AspNetCore.Mvc;
namespace LabTrack.Controllers;
public class HomeController:Controller {public IActionResult Index()=>RedirectToAction("Index","Equipos");public IActionResult Error()=>View();}
