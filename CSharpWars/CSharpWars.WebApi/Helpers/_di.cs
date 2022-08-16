namespace CSharpWars.WebApi.Helpers;

public static class ServiceCollectionExtensions
{
    public static void AddHelpers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IApiHelper<>), typeof(ApiHelper<>));
        services.AddScoped(typeof(IClusterClientHelper<>), typeof(ClusterClientHelper<>));
        services.AddScoped(typeof(IClusterClientHelperWithGuidKey<>), typeof(ClusterClientHelperWithGuidKey<>));
        services.AddScoped(typeof(IClusterClientHelperWithStringKey<>), typeof(ClusterClientHelperWithStringKey<>));
    }
}