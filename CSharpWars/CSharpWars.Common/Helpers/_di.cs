using CSharpWars.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpWars.Common.Helpers;

public static class ServiceCollectionExtensions
{
    public static void AddCommonHelpers(this IServiceCollection services)
    {
        services.AddSingleton<IJwtHelper, JwtHelper>();
        services.AddSingleton<IPasswordHashHelper, PasswordHashHelper>();
    }
}