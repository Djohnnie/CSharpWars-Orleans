using Microsoft.Extensions.DependencyInjection;

namespace CSharpWars.Logic;

public static class ServiceCollectionExtensions
{
    public static void AddLogic(this IServiceCollection services)
    {
        services.AddSingleton<IBotLogic, BotLogic>();
    }
}