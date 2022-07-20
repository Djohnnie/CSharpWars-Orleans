using CSharpWars.Common.Helpers;
using CSharpWars.WebApi.Security;
using System.Net;

namespace CSharpWars.WebApi.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtHelper _jwtHelper;

    public JwtMiddleware(RequestDelegate next, IJwtHelper jwtHelper)
    {
        _next = next;
        _jwtHelper = jwtHelper;
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

        if (endpoint.Metadata.Any(x => x.GetType() == typeof(JwtAuthorization)))
        {

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var userName = _jwtHelper.ValidateToken(token);

            playerContext.PlayerName = userName;

            if (userName == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Token Validation Has Failed. Request Access Denied");
                return;
            }
        }

        await _next(context);
    }
}