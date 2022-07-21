using CSharpWars.WebApi.Middleware;

namespace CSharpWars.WebApi.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapAuthorizedPost(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
    {
        return endpoints.MapPost(pattern, handler).WithMetadata(JwtAuthorization.Instance);
    }

    public static IEndpointConventionBuilder MapAdminDelete(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
    {
        return endpoints.MapDelete(pattern, handler).WithMetadata(JwtAdminAuthorization.Instance);
    }
}