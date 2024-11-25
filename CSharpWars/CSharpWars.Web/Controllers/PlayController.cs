using System.Text;
using CSharpWars.Common.Extensions;
using CSharpWars.Orleans.Contracts;
using CSharpWars.Web.Client;
using CSharpWars.Web.Extensions;
using CSharpWars.Web.Models;
using CSharpWars.WebApi.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CSharpWars.Web.Controllers;

public class PlayController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IOrleansClient _orleansClient;

    public PlayController(
        IConfiguration configuration,
        IOrleansClient orleansClient)
    {
        _configuration = configuration;
        _orleansClient = orleansClient;
    }

    public IActionResult Template()
    {
        if (HttpContext.Session.Keys.Contains("PLAYER"))
        {
            var player = HttpContext.Session.GetObject<LoginResponse>("PLAYER");
            var vm = new PlayViewModel
            {
                PlayerName = player.Username,
                BotName = "<name your bot>",
                BotHealth = 100,
                BotStamina = 100,
                Scripts = Templates.All
            };
            ViewData["ArenaUrl"] = _configuration.GetValue<string>("ARENA_URL");
            ViewData["ScriptTemplateUrl"] = _configuration.GetValue<string>("SCRIPT_TEMPLATE_URL");
            return View(vm);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Template(PlayViewModel vm)
    {
        if (HttpContext.Session.Keys.Contains("PLAYER"))
        {
            var player = HttpContext.Session.GetObject<LoginResponse>("PLAYER");

            var (valid, sadMessage) = IsValid(vm);

            if (valid)
            {
                var script = Templates.All.Single(x => x.Id == vm.SelectedScript).Script.Base64Encode();

                var validatedScript = await _orleansClient.Validate(new ScriptToValidateDto { Script = script });

                if (validatedScript != null && validatedScript.ValidationMessages.Count == 0)
                {
                    var botToCreate = new BotToCreateDto
                    {
                        PlayerName = player.Username,
                        BotName = vm.BotName,
                        ArenaName = "default",
                        MaximumHealth = vm.BotHealth,
                        MaximumStamina = vm.BotStamina,
                        Script = script
                    };

                    try
                    {
                        await _orleansClient.CreateBot(player.Username, botToCreate);
                    }
                    catch (Exception ex)
                    {
                        valid = false;
                        sadMessage = ex.Message;
                    }
                }
                else
                {
                    valid = false;
                    if (validatedScript == null)
                    {
                        sadMessage = "Your script could not be validated for an unknown reason.";
                    }
                    else
                    {
                        var scriptErrors = string.Join(", ", validatedScript.ValidationMessages.Select(x => x.Message));
                        sadMessage = $"Your script contains some compile errors: {scriptErrors}";
                    }
                }
            }

            vm = new PlayViewModel
            {
                PlayerName = player.Username,
                BotName = vm.BotName,
                BotHealth = vm.BotHealth,
                BotStamina = vm.BotStamina,
                SelectedScript = vm.SelectedScript,
                Scripts = Templates.All
            };

            if (valid)
            {
                vm.HappyMessage = $"{vm.BotName} for player {vm.PlayerName} has been created successfully!";
            }
            else
            {
                vm.SadMessage = sadMessage;
            }

            ViewData["ArenaUrl"] = _configuration.GetValue<string>("ARENA_URL");
            ViewData["ScriptTemplateUrl"] = _configuration.GetValue<string>("SCRIPT_TEMPLATE_URL");
            return View(vm);
        }

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Custom()
    {
        if (HttpContext.Session.Keys.Contains("PLAYER"))
        {
            var player = HttpContext.Session.GetObject<LoginResponse>("PLAYER");
            var vm = new PlayViewModel
            {
                PlayerName = player.Username,
                BotName = "<name your bot>",
                BotHealth = 100,
                BotStamina = 100,
                Script = Templates.WalkAround
            };
            ViewData["ArenaUrl"] = _configuration.GetValue<string>("ARENA_URL");
            ViewData["ScriptTemplateUrl"] = _configuration.GetValue<string>("SCRIPT_TEMPLATE_URL");
            return View(vm);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Custom(PlayViewModel vm)
    {
        if (HttpContext.Session.Keys.Contains("PLAYER"))
        {
            var player = HttpContext.Session.GetObject<LoginResponse>("PLAYER");

            vm = new PlayViewModel
            {
                PlayerName = player.Username,
                BotName = vm.BotName,
                BotHealth = vm.BotHealth,
                BotStamina = vm.BotStamina,
                Script = vm.Script
            };

            try
            {
                var (valid, sadMessage) = IsValid(vm);

                if (valid)
                {
                    var script = vm.Script.Base64Encode();

                    var validatedScript = await _orleansClient.Validate(new ScriptToValidateDto { Script = script });

                    if (validatedScript != null && validatedScript.ValidationMessages.Count == 0)
                    {
                        var botToCreate = new BotToCreateDto
                        {
                            PlayerName = player.Username,
                            BotName = vm.BotName,
                            ArenaName = "default",
                            MaximumHealth = vm.BotHealth,
                            MaximumStamina = vm.BotStamina,
                            Script = script
                        };

                        try
                        {
                            await _orleansClient.CreateBot(player.Username, botToCreate);
                        }
                        catch (Exception ex)
                        {
                            valid = false;
                            sadMessage = ex.Message;
                        }
                    }
                    else
                    {
                        valid = false;
                        if (validatedScript == null)
                        {
                            sadMessage = "Your script could not be validated for an unknown reason.";
                        }
                        else
                        {
                            var scriptErrors = string.Join(", ", validatedScript.ValidationMessages.Select(x => x.Message));
                            sadMessage = $"Your script contains some compile errors: {scriptErrors}";
                        }
                    }
                }

                if (valid)
                {
                    vm.HappyMessage = $"{vm.BotName} for player {vm.PlayerName} has been created successfully!";
                }
                else
                {
                    vm.SadMessage = sadMessage;
                }
            }
            catch (Exception ex)
            {
                vm.SadMessage = $"{ex}";
            }

            ViewData["ArenaUrl"] = _configuration.GetValue<string>("ARENA_URL");
            ViewData["ScriptTemplateUrl"] = _configuration.GetValue<string>("SCRIPT_TEMPLATE_URL");
            return View(vm);
        }

        return RedirectToAction("Index", "Home");
    }

    private (bool, string) IsValid(PlayViewModel vm)
    {
        var validBotName = !string.IsNullOrEmpty(vm.BotName);
        var validHealthAndStamina = vm.BotHealth > 0 && vm.BotStamina > 0 && vm.BotHealth + vm.BotStamina <= _configuration.GetValue<int>("POINTS_LIMIT");
        var validScript = vm.SelectedScript != Guid.Empty || !string.IsNullOrEmpty(vm.Script);

        var sadMessage = new StringBuilder();
        if (!validBotName || !validHealthAndStamina || !validScript)
        {
            sadMessage.AppendLine("You have made some errors:");

            if (!validBotName)
            {
                sadMessage.AppendLine(" - Your robot name cannot be empty");
            }

            if (!validHealthAndStamina)
            {
                sadMessage.AppendLine(" - Your health and stamina are not valid");
            }

            if (!validScript)
            {
                sadMessage.AppendLine(" - Your script cannot be empty");
            }
        }

        return (validBotName && validHealthAndStamina && validScript, sadMessage.ToString());
    }
}