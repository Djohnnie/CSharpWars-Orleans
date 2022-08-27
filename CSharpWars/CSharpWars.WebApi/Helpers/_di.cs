namespace CSharpWars.WebApi.Helpers;

public static class ServiceCollectionExtensions
{
    public static void AddHelpers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IApiHelper<>), typeof(ApiHelper<>));
    }
}