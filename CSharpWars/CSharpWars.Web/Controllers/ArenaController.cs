using Microsoft.AspNetCore.Mvc;

namespace CSharpWars.Web.Controllers;

public class ArenaController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}