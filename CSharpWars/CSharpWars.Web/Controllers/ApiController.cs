using CSharpWars.Scripting;
using CSharpWars.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System.ComponentModel;
using System.Reflection;

namespace CSharpWars.Web.Controllers;

[Route("api")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ApiController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("script")]
    public async Task<IActionResult> GenerateScript([FromQuery] string prompt)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return BadRequest();
            }

            var (arenaWidth, arenaHeight) = (_configuration.GetValue<int>("ARENA_WIDTH"), _configuration.GetValue<int>("ARENA_HEIGHT"));

            var kernel = InitializeSemanticKernel();
            var result = await kernel.InvokeAsync("csharpwarsScript", "csharpwarsScript", new() { { "request", prompt },
                                                                         { "csharpwarsFunctions", GetCSharpWarsFunctions()},
                                                                         { "csharpwarsProperties", GetCSharpWarsProperties()},
                                                                         { "arenaDimensions", $"({arenaWidth}, {arenaHeight})"},
                                                                         { "walkAroundTemplate", Templates.WalkAround},
                                                                         { "walkBackAndForthTemplate", Templates.WalkBackAndForth},
                                                                         { "lookAroundAndRangeAttackTemplate", Templates.LookAroundAndRangeAttack},
                                                                         { "lookAroundAndSelfDestructTemplate", Templates.LookAroundAndSelfDestruct},
                                                                         { "huntDownTemplate", Templates.HuntDown} });

            var script = result.GetValue<string>();

            return Ok(script);
        }
        catch (Exception ex)
        {
            return Ok($"{ex}");
        }
    }

    private Kernel InitializeSemanticKernel()
    {
        var builder = Kernel.CreateBuilder();

        var deploymentName = _configuration.GetValue<string>("AZURE_OPEN_AI_DEPLOYMENT_NAME");
        var endpoint = _configuration.GetValue<string>("AZURE_OPEN_AI_ENDPOINT");
        var apiKey = _configuration.GetValue<string>("AZURE_OPEN_AI_APIKEY");

        builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);

        var kernel = builder.Build();

        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CSharpWars.Web.csharpwars.prompt.yaml")!;
        using StreamReader reader = new(stream);

        KernelFunction csharpwarsScript = kernel.CreateFunctionFromPromptYaml(
            reader.ReadToEnd(),
            promptTemplateFactory: new HandlebarsPromptTemplateFactory()
        );

        kernel.Plugins.AddFromFunctions("csharpwarsScript", [csharpwarsScript]);

        return kernel;
    }

    private List<string> GetCSharpWarsFunctions()
    {
        var scriptGlobalsType = typeof(ScriptGlobals);
        var publicMethods = scriptGlobalsType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        var functionsOverview = new List<string>();

        foreach (var publicMethod in publicMethods)
        {
            var descriptionAttribute = publicMethod.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
            {
                functionsOverview.Add("/// <summary>");
                functionsOverview.Add($"/// {descriptionAttribute.Description}");
                functionsOverview.Add("/// </summary>");
                functionsOverview.Add(publicMethod.ToString());
                functionsOverview.Add(string.Empty);
            }

        }

        return functionsOverview;
    }

    private List<string> GetCSharpWarsProperties()
    {
        var scriptGlobalsType = typeof(ScriptGlobals);
        var publicProperties = scriptGlobalsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var propertiesOverview = new List<string>();

        foreach (var publicProperty in publicProperties)
        {
            var descriptionAttribute = publicProperty.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
            {
                propertiesOverview.Add("/// <summary>");
                propertiesOverview.Add($"/// {descriptionAttribute.Description}");
                propertiesOverview.Add("/// </summary>");
                propertiesOverview.Add($"{publicProperty} {{get;}}");
                propertiesOverview.Add(string.Empty);
            }
        }

        return propertiesOverview;
    }
}