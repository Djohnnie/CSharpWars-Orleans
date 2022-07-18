namespace CSharpWars.WebApi.Managers;

public static class ServiceCollectionExtensions
{
    public static void AddManagers(this IServiceCollection services)
    {
        services.AddScoped<IStatusManager, StatusManager>();
        services.AddScoped<IPlayerManager, PlayerManager>();
        services.AddScoped<IArenaManager, ArenaManager>();
    }
}