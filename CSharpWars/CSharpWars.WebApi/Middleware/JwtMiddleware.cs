using CSharpWars.Common.Helpers;
using CSharpWars.WebApi.Security;
using System.Net;

namespace CSharpWars.WebApi.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtHelper _jwtHelper;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(
        RequestDelegate next,
        IJwtHelper jwtHelper,
        IConfiguration configuration)
    {
        _next = next;
        _jwtHelper = jwtHelper;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context, IPlayerContext playerContext)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync("Endpoint was not found!");
            return;
        }

        if (endpoint.Metadata.Any(x => x.GetType() == typeof(JwtAuthorization) || x.GetType() == typeof(JwtAdminAuthorization)))
        {
            var authorizationToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var playerName = _jwtHelper.ValidateToken(authorizationToken);

            playerContext.PlayerName = playerName;

            if (playerName == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Token Validation Has Failed. Request Access Denied");
                return;
            }
        }

        if (endpoint.Metadata.Any(x => x.GetType() == typeof(JwtAdminAuthorization)))
        {
            var adminKey = context.Request.Headers["AdminKey"].FirstOrDefault()?.Split(" ").Last();

            if (_configuration.GetValue<string>("ADMIN_KEY") != adminKey)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("No AdminKey provided. Request Access Denied");
                return;
            }
        }

        await _next(context);
    }
}