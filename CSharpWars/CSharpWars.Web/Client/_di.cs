namespace CSharpWars.Web.Client;

public static class ServiceCollectionExtensions
{
    public static void AddClients(this IServiceCollection services)
    {
        services.AddScoped<IOrleansClient, OrleansClient>();
    }
}