using CSharpWars.Orleans.Contracts;
using CSharpWars.Web.Client;
using CSharpWars.Web.Extensions;
using CSharpWars.Web.Models;
using CSharpWars.WebApi.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CSharpWars.Web.Controllers;

public class HomeController : Controller
{
    private readonly IOrleansClient _orleansClient;
    private readonly IConfiguration _configuration;

    public HomeController(
        IOrleansClient orleansClient,
        IConfiguration configuration)
    {
        _orleansClient = orleansClient;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.Keys.Contains("PLAYER"))
        {
            return RedirectToAction(nameof(Play));
        }

        var vm = new PlayerViewModel();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Index(PlayerViewModel vm)
    {
        try
        {
            var player = await _orleansClient.Login(vm.Name, vm.Secret);
            if (player != null)
            {
                HttpContext.Session.SetObject("PLAYER", player);
                return RedirectToAction(nameof(Play));
            }
        }
        catch (Exception ex)
        {
            vm.Message = ex.Message;
        }

        return View(vm);
    }

    public IActionResult Play()
    {
        if (HttpContext.Session.Keys.Contains("PLAYER"))
        {
            var player = HttpContext.Session.GetObject<PlayerDto>("PLAYER");
            var vm = new GameViewModel
            {
                PlayerName = player.Username,
                SampleScript = Templates.WalkAround,
                IsTemplatePlayEnabled = _configuration.GetValue<bool>("ENABLE_TEMPLATE_PLAY"),
                IsCustomPlayEnabled = _configuration.GetValue<bool>("ENABLE_CUSTOM_PLAY")
            };
            ViewData["ArenaUrl"] = _configuration.GetValue<string>("ARENA_URL");
            ViewData["ScriptTemplateUrl"] = _configuration.GetValue<string>("SCRIPT_TEMPLATE_URL");
            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult LogOut()
    {
        if (HttpContext.Session.Keys.Contains("PLAYER"))
        {
            HttpContext.Session.Remove("PLAYER");
        }

        return RedirectToAction(nameof(Index));
    }
}