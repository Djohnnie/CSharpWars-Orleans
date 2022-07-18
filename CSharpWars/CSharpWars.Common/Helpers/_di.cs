using Microsoft.Extensions.DependencyInjection;

namespace CSharpWars.Helpers;

public static class ServiceCollectionExtensions
{
    public static void AddCommonHelpers(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHashHelper, PasswordHashHelper>();
    }
}