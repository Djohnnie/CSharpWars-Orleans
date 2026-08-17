using Microsoft.AspNetCore.Mvc;

namespace CSharpWars.Web.Controllers;

public class ArenaController : Controller
{
    private const string ApiBaseAddressKey = "API_BASE_ADDRESS";
    private readonly IConfiguration _configuration;

    public ArenaController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        var configuredAddress = _configuration[ApiBaseAddressKey];
        if (!Uri.TryCreate(configuredAddress, UriKind.Absolute, out var apiBaseAddress)
            || (apiBaseAddress.Scheme != Uri.UriSchemeHttp && apiBaseAddress.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                $"{ApiBaseAddressKey} must be configured with an absolute HTTP or HTTPS address.");
        }

        ViewData["ApiBaseAddress"] = apiBaseAddress.AbsoluteUri.TrimEnd('/');
        return View();
    }
}