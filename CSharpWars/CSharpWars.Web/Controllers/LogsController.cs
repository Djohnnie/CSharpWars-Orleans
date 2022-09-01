using CSharpWars.Orleans.Contracts;
using CSharpWars.Web.Client;
using CSharpWars.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CSharpWars.Web.Controllers;

public class LogsController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IOrleansClient _orleansClient;

    public LogsController(
        IConfiguration configuration,
        IOrleansClient orleansClient)
    {
        _configuration = configuration;
        _orleansClient = orleansClient;
    }

    public async Task<IActionResult> Index()
    {
        await Task.Delay(1);

        var vm = new LogsViewModel
        {
            Moves = new MoveLogsViewModel { Items = new List<string>() },
            Messages = new MessageLogsViewModel { Items = new List<string>() }
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Moves()
    {
        var moves = await _orleansClient.GetMoves("default");

        var vm = new MoveLogsViewModel
        {
            Items = moves.OrderByDescending(x => x.TimeStamp).Select(SelectMoves()).ToList()
        };

        return PartialView("_Moves", vm);
    }

    [HttpGet]
    public async Task<IActionResult> Messages()
    {
        var messages = await _orleansClient.GetMessages("default");

        var vm = new MessageLogsViewModel
        {
            Items = messages.OrderByDescending(x => x.TimeStamp).Select(SelectMessages()).ToList()
        };

        return PartialView("_Messages", vm);
    }

    private static Func<MoveDto, string> SelectMoves()
    {
        return x => $"{x.TimeStamp:HH:mm:ss} | {x.Owner} | {x.Description}";
    }

    private static Func<MessageDto, string> SelectMessages()
    {
        return x => $"{x.TimeStamp:HH:mm:ss} | {x.Owner} | {x.Message}";
    }
}