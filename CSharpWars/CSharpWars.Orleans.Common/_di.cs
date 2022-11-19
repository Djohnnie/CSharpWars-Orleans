using Microsoft.Extensions.DependencyInjection;

namespace CSharpWars.Orleans.Common;

public static class ServiceCollectionExtensions
{
    public static void AddOrleansHelpers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGrainFactoryHelper<>), typeof(GrainFactoryHelper<>));
        services.AddScoped(typeof(IGrainFactoryHelperWithGuidKey<>), typeof(GrainFactoryHelperWithGuidKey<>));
        services.AddScoped(typeof(IGrainFactoryHelperWithStringKey<>), typeof(GrainFactoryHelperWithStringKey<>));
    }
}